namespace Library

open System
open System.Globalization
open System.Windows
open System.Windows.Controls
open System.Windows.Data

type StringValue () =
    let mutable s = ""
    member this.Value with get () = s and set v = s <- v

type CustomValidationRule (validator:string -> string) =
    inherit ValidationRule ()
    override x.Validate(value:obj, culture:CultureInfo) =
        let s = value :?> string
        match s |> validator with
        | null | "" ->  ValidationResult.ValidResult
        | error -> ValidationResult(false, error)

[<Sealed>]
type Form private () =    
    static let mutable validate : obj seq -> bool = fun _ -> true
    static let mutable submit : obj seq -> unit = ignore
    static let mutable answers = ResizeArray()
    static let createForm () =
        My.App.Dispatcher.Invoke(System.Func<_>(fun () ->
            GraphicsWindow.Show()
            let table = Grid(Margin=Thickness(8.0))
            let addColumn width =
                table.ColumnDefinitions.Add(ColumnDefinition(Width=width))
            GridLength(2.0,GridUnitType.Star) |> addColumn
            GridLength(1.0,GridUnitType.Star) |> addColumn
            My.App.Window.Content <- table
            let button = Button(Content="Submit")
            button.Click.Add(fun _ -> answers |> Seq.map (fun f -> f()) |> submit)
            box (table, button)
        )) :?> Grid * Button
    static let form = lazy (createForm())   
    static let mutable rowIndex = 0
    static let margin = Thickness(4.0)
    static let addRow(rowFun:Grid -> unit) =
        My.App.Dispatcher.Invoke(Action(fun () ->
            let table,submit = form.Force()
            table.RowDefinitions.Add(RowDefinition())
            if rowIndex = 0 then
                table.RowDefinitions.Add(RowDefinition())
                Grid.SetColumnSpan(submit, 2)
                table.Children.Add(submit) |> ignore
            Grid.SetRow(submit,rowIndex+1)
            rowFun table
            rowIndex <- rowIndex + 1
        )) |> ignore
    static let addQuestion(label, create:unit -> #FrameworkElement * (unit -> obj)) =
        let controls (table:Grid)  =
            let question = TextBlock(Text=label, Margin=margin)
            Grid.SetRow(question, rowIndex)
            table.Children.Add question |> ignore
            let control, read = create()
            control.Margin <- margin
            Grid.SetColumn(control, 1)
            Grid.SetRow(control, rowIndex)
            table.Children.Add control |> ignore
            answers.Add read
        addRow controls
    static member Title
        with get () = GraphicsWindow.Title
        and set(value) = GraphicsWindow.Title <- value
    static member Clear() =
        rowIndex <- 0      
        answers.Clear()
        My.App.Dispatcher.Invoke(Action(fun _ ->
            let table, _ = form.Force()
            table.RowDefinitions.Clear()
            table.Children.Clear()
        )) |> ignore
    static member Group(group) =
        ()
    static member Information(label) =
        let create (table:Grid) = 
            let control = TextBlock(Text=label, Margin=margin)
            Grid.SetRow(control, rowIndex)
            Grid.SetColumnSpan(control, 2)
            table.Children.Add control |> ignore 
        addRow create
    static member Question(label, isValidCallback:string -> string) =
        let create () =
            let control = TextBox()
            control.DataContext <- StringValue()
            let binding = Binding "Value"
            binding.UpdateSourceTrigger <- UpdateSourceTrigger.PropertyChanged
            binding.Mode <- BindingMode.TwoWay
            binding.ValidationRules.Add(CustomValidationRule(isValidCallback))
            control.SetBinding(TextBox.TextProperty, binding) |> ignore
            control, fun () -> box control.Text
        addQuestion(label,create)
    static member Question(label) =
        Form.Question(label, (fun _ -> null))
    static member NumericalQuestion(label, isValidCallback:string -> string) =        
        Form.Question(label, fun s -> s.Trim() |> isValidCallback)
    static member Options(label, options:string seq, selectedItem) =
        let createCombo () =
            let answer = ComboBox()
            options |> Seq.iter (answer.Items.Add >> ignore)
            answer.SelectedItem <- selectedItem
            answer, fun () -> answer.SelectedItem
        let createRadio () =             
            let panel = StackPanel()
            options |> Seq.iter (fun s -> 
                let isChecked = System.Nullable<_>((s=selectedItem))
                RadioButton(Content=s,GroupName=label,IsChecked=isChecked) 
                |> (panel.Children.Add >> ignore))
            panel, fun () -> box selectedItem
        addQuestion(label,createCombo)
    static member Options(label, options:string seq) =
        Form.Options(label, options, options.First())
    static member Options(label, options:int seq, selectedItem) =
        let create () =
            let answer = ComboBox()
            options |> Seq.iter (answer.Items.Add >> ignore)
            answer.SelectedItem <- selectedItem
            answer, fun () -> answer.SelectedItem
        addQuestion(label,create)
    static member Options(label, options:int seq) =
        Form.Options(label, options, options.First())
    static member Choice(label) =        
        let create () =
            let answer = CheckBox()
            answer, fun () -> box answer.IsChecked
        addQuestion(label,create)
    static member OnValidate (validateCallback:obj seq -> bool) =
        validate <- validateCallback
    static member OnSubmit (submitCallback:obj seq -> unit) =
        submit <- submitCallback