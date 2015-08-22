namespace Library

open System
open System.Windows.Controls

[<Sealed>]
type Controls private () =
    static let mutable onClick = Callback(ignore)
    static let mutable lastClickedButton = ""
    static let controls = System.Collections.Generic.Dictionary<string,Control>()
    static let buttonNo = ref 0
    static let textBoxNo = ref 0
    static member AddButton(caption,x,y) = 
        incr textBoxNo
        let name = "TextBox" + (!textBoxNo).ToString()        
        My.App.Dispatcher.Invoke(Action(fun () ->
            let button = Button(Content=caption)
            button.Click.Add(fun args ->
                lastClickedButton <- name
                onClick.Invoke()
            )       
            Canvas.SetLeft(button,x)
            Canvas.SetTop(button,y)
            controls.Add(name,button)
            My.App.Canvas.Children.Add(button) |> ignore
        )) |> ignore
        name
    static member AddButton(caption,x:int,y:int) =
        Controls.AddButton(caption,float x,float y)
    static member AddTextBox(x,y) =
        incr textBoxNo
        let name = "TextBox" + (!textBoxNo).ToString()
        My.App.Dispatcher.Invoke(Action(fun () ->
            let textBox = TextBox()
            Canvas.SetLeft(textBox,x)
            Canvas.SetTop(textBox,y)
            controls.Add(name,textBox)
            My.App.Canvas.Children.Add(textBox) |> ignore
        )) |> ignore
        name
    static member AddTextBox(x:int,y:int) =
        Controls.AddTextBox(float x, float y)
    static member SetSize(controlName,width,height) =
        match controls.TryGetValue(controlName) with
        | true, control ->
            My.App.Dispatcher.Invoke(Action(fun () ->
                control.Width <- width
                control.Height <- height
            )) |> ignore
        | _ -> ()
    static member SetSize(controlName,width:int,height:int) =
        Controls.SetSize(controlName,float width,float height)
    static member GetTextBoxText(controlName) =
        match controls.TryGetValue(controlName) with
        | true, control ->
            My.App.Dispatcher.Invoke(Func<_>(fun () ->
                let textBox = control :?> TextBox
                textBox.Text
            )) :?> string
        | _ -> ""
    static member SetTextBoxText(controlName,text) =
        match controls.TryGetValue(controlName) with
        | true, control ->
            My.App.Dispatcher.Invoke(Action(fun () ->
                let textBox = control :?> TextBox
                textBox.Text <- text
            )) |> ignore
        | _ -> ()
    static member LastClickedButton = lastClickedButton
    static member ButtonClicked with set handler = onClick <- handler