namespace Library

open System
open System.Windows
open System.Windows.Controls
open System.Threading

type Callback = delegate of unit -> unit

type internal App () =
    let mutable app : Application = null
    let mutable window : Window = null
    let mutable canvas : Canvas = null
    let mutable turtle : Image = null
    let mutable mousePosition = 0.0,0.0
    let mutable isLeftButtonDown = false
    let mutable isRightButtonDown = false
    let mutable mouseMove = Callback(ignore)
    let mutable mouseDown = Callback(ignore)
    let mutable mouseUp = Callback(ignore)
    let mutable lastKey = ""
    let mutable keyDown = Callback(ignore)
    let mutable keyUp = Callback(ignore)
    let mutable update = Callback(ignore)
    let createTurtle () =
        let bitmap = Media.Imaging.BitmapImage()
        bitmap.BeginInit()
        bitmap.StreamSource <- typeof<App>.Assembly.GetManifestResourceStream("turtle.png")
        bitmap.EndInit()
        bitmap
    let createWindow () =   
        let win = Window(SizeToContent=SizeToContent.WidthAndHeight)
        win.Closed |> Event.add (fun args ->
            Program.End()
        )
        canvas <- Canvas(MinWidth=512.0,MinHeight=384.0)        
        turtle <- Image(Source=createTurtle ())
        turtle.Visibility <- Visibility.Collapsed
        Canvas.SetZIndex(turtle,1)
        canvas.Children.Add(turtle) |> ignore
        win.Content <- canvas
        win.MouseMove |> Event.add (fun e ->
            mousePosition <- let p = e.GetPosition(win) in p.X, p.Y
            mouseMove.Invoke()
        )
        win.MouseDown |> Event.add (fun args -> 
            isLeftButtonDown <- args.LeftButton = Input.MouseButtonState.Pressed
            isRightButtonDown <- args.RightButton  = Input.MouseButtonState.Pressed
            mouseDown.Invoke()
        )
        win.MouseUp |> Event.add (fun args ->
            isLeftButtonDown <- args.LeftButton = Input.MouseButtonState.Pressed
            isRightButtonDown <- args.RightButton  = Input.MouseButtonState.Pressed
            mouseUp.Invoke()
        )
        win.KeyDown |> Event.add (fun e ->
            lastKey <- e.Key.ToString()
            keyDown.Invoke()
        )
        win.KeyUp |> Event.add (fun e -> 
            keyUp.Invoke()
        )
        System.Windows.Media.CompositionTarget.Rendering.Add (fun e ->
            update.Invoke()
        )
        win
    do  use autoEvent = new AutoResetEvent(false)
        let thread =
            new Thread(fun () ->
                app <- Application()
                window <- createWindow()
                autoEvent.Set() |> ignore
                app.Run(window) |> ignore
            )
        thread.SetApartmentState(ApartmentState.STA)
        thread.Start()
        autoEvent.WaitOne() |> ignore
    member __.Dispatcher = app.Dispatcher
    member __.Window = window
    member __.Canvas = canvas
    member __.Turtle = turtle
    member __.MouseDown with set handler = mouseDown <- handler
    member __.MouseUp with set handler = mouseUp <- handler
    member __.MouseMove with set handler = mouseMove <- handler
    member __.MousePosition with get () = mousePosition
    member __.IsLeftButtonDown with get () = isLeftButtonDown
    member __.IsRightButtonDown with get () = isRightButtonDown
    member __.KeyUp with set handler = keyUp <- handler
    member __.KeyDown with set handler = keyDown <- handler
    member __.LastKey with get () = lastKey
    member __.Update with set handler = update <- handler

type [<Sealed>] internal My private () =
    static let mutable app = None
    static let sync = obj ()
    static let createApp () =
        let newApp = new App()
        app <- Some newApp
        newApp
    static member App =
        lock(sync) (fun () ->
            match app with
            | Some app -> app
            | None -> createApp()
        )

