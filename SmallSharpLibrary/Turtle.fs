namespace Library

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes

[<Sealed>]
type Turtle private () =
    static let mutable isHidden = false
    static let mutable isVisible = false
    static let mutable isPenDown = true
    static let mutable x = 256.0
    static let mutable y = 192.0
    static let mutable a = 0.0
    static let dispatch f =
        My.App.Dispatcher.BeginInvoke(Action(f)) |> ignore
    static let showTurtle () =
        if not isVisible then
            isVisible <- true
            if not isHidden then
                dispatch (fun () -> My.App.Turtle.Visibility <- Visibility.Visible)
    static let hideTurtle () =
        if isVisible then
            isVisible <- false
            dispatch (fun () -> My.App.Turtle.Visibility <- Visibility.Collapsed)
    static let moveTurtle () =
        dispatch (fun () ->
            Canvas.SetLeft(My.App.Turtle, x - 8.0)
            Canvas.SetTop(My.App.Turtle, y - 8.0)
        )
        showTurtle()
    static let rotateTurtle () = 
        dispatch (fun () ->
            let element = My.App.Turtle
            let transform =
                match element.RenderTransform  with
                | :? Media.RotateTransform as t -> t
                | _ ->                     
                    let transform = Media.RotateTransform()
                    element.RenderTransform <- transform
                    transform
            let fe = element :> FrameworkElement
            transform.CenterX <- fe.ActualWidth / 2.0
            transform.CenterY <- fe.ActualHeight / 2.0
            transform.Angle <- a
        )
        showTurtle()
    static member Angle
        with get () = a
        and set degrees = 
            a <- degrees
            rotateTurtle()
    static member X
        with get () = x
        and set value = 
            x <- value
            moveTurtle()
    static member Y
        with get () = y
        and set value = 
            y <- value
            moveTurtle()
    static member Turn(degrees:float) =
        a <- a + degrees
        rotateTurtle()
    static member Turn(degrees:int) =
        Turtle.Turn(float degrees)
    static member TurnLeft() =
        Turtle.Turn(-90.0)
    static member TurnRight() =
        Turtle.Turn(90.0)
    static member Move(distance:float) =
        let r = (a - 90.0) * System.Math.PI / 180.0
        let x2 = x + distance * cos r
        let y2 = y + distance * sin r        
        if isPenDown then GraphicsWindow.DrawLine(x,y,x2,y2)
        x <- x2
        y <- y2       
        moveTurtle()
    static member Move(distance:int) =
        Turtle.Move(float distance)
    static member MoveTo(x,y) =
        Turtle.X <- x
        Turtle.Y <- y 
        moveTurtle()
    static member MoveTo(x:int,y:int) =        
        Turtle.MoveTo(float x, float y)
    static member PenUp() =
        isPenDown <- false
    static member PenDown() =
        isPenDown <- true
    static member Show() =
        isHidden <- false
        showTurtle()
    static member Hide() =
        isHidden <- true
        hideTurtle()
        

[<AutoOpen>]
module turtle =
    type private distance = int
    type private angle = int
    let forward (distance:distance) = Turtle.Move distance
    let fd (distance:distance) = Turtle.Move distance
    let left (degrees:angle) = Turtle.Turn -degrees
    let lt (degrees:angle) = left degrees
    let right (degrees:angle) = Turtle.Turn degrees
    let rt (degrees:angle) = right degrees
    let pencolor (color:Color) = GraphicsWindow.PenColor <- color
    let penup () = Turtle.PenUp()
    let pendown () = Turtle.PenDown()
    let repeat n f = for i = 1 to n do f ()