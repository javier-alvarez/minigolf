
#if INTERACTIVE
#r @"C:\Users\jaalvare\Desktop\smallsharp_fc47fc87aa98\SmallSharpLibrary\bin\Debug\SmallSharpLibrary.dll"
#endif


open System
open Library

[<AutoOpen>]
module MathAngles = 
    // Math
    [<Measure>]
    type degree
    
    [<Measure>]
    type radian
    
    let convertToRadian (degree : float<degree>) : float<radian> = degree * Math.PI / 180.<degree/radian>

type Line = 
    { X1 : float
      Y1 : float
      X2 : float
      Y2 : float }
    member this.Add = Shapes.AddLine(this.X1, this.Y1, this.X2, this.Y2)

type Arrow = 
    { Line : Line
      LineName : string
      Angle : float<degree> }
    
    member this.Rotate angleDegree = 
        GraphicsWindow.BrushColor <- Colors.Black
        GraphicsWindow.PenColor <- Colors.Black
        this.Hide ()
        let angleDegree = (this.Angle + angleDegree) % 360.<degree>
        let angle = convertToRadian angleDegree
        let xlength = abs (this.Line.X2 - this.Line.X1)
        let ylength = abs (this.Line.Y2 - this.Line.Y1)
        let length = Math.Sqrt((xlength * xlength) + (ylength * ylength))
        let x2 = this.Line.X1 + (Math.Cos(float angle)) * length
        let y2 = this.Line.Y1 + (Math.Sin(float angle)) * length
        
        let line = 
            { this.Line with X2 = x2
                             Y2 = y2 }
        
        let lineName = line.Add
        { Line = line
          LineName = lineName
          Angle = angleDegree }
    
    member this.Hide () = Shapes.HideShape this.LineName
    member this.MoveLeft () = this.Rotate -1.<degree>
    member this.MoveRight () = this.Rotate 1.<degree>

type Direction = 
    | Left
    | Right

type GameState = 
    | Uninitialised
    | Started of hole : string * ball : string * arrow : Arrow
    | ArrowMove of hole : string * ball : string * arrow : Arrow * direction : Direction
    | BallFire of hole : string * ball : string * arrow : Arrow * lastRendered : int
    | End
    with override this.ToString() =
        match this with 
        | Uninitialised -> "Uninitialised"
        | Started _ -> "Started"
        | ArrowMove _ -> "ArrowMove"
        | BallFire _ -> "BallFire"
        | End -> "End"

let initGame() : GameState = 
    let sizeOfShapes = 16.
    //GraphicsWindow.Show()
    GraphicsWindow.Clear()
    GraphicsWindow.BackgroundColor <- Colors.Green
    // Ball
    let ball = 
        GraphicsWindow.BrushColor <- Colors.White
        GraphicsWindow.PenColor <- Colors.Black
        let ball = Shapes.AddEllipse(sizeOfShapes, sizeOfShapes)
        Shapes.Move(ball, GraphicsWindow.Width / 2, (GraphicsWindow.Height / 4) * 3)
        ball
    
    // Arrow
    let initialArrow = 
        GraphicsWindow.BrushColor <- Colors.Black
        GraphicsWindow.PenColor <- Colors.Black
        let xball = Shapes.GetLeft(ball) + sizeOfShapes / 2.
        let yball = Shapes.GetTop(ball)
        let ylineEnd = yball - 50.
        
        let line = 
            { Line.X1 = xball
              Y1 = yball
              X2 = xball
              Y2 = ylineEnd }
        
        let lineName = line.Add
        { LineName = lineName
          Line = line
          Angle = 270.<degree> }
    
    // Create hole
    let hole = 
        GraphicsWindow.BrushColor <- Colors.Gray
        GraphicsWindow.PenColor <- Colors.White
        let hole = Shapes.AddEllipse(sizeOfShapes, sizeOfShapes)
        Shapes.Move(hole, GraphicsWindow.Width / 2, GraphicsWindow.Height / 4)
        hole
    
    Started(hole, ball, initialArrow)

// Move the ball
let AnimateBallMove(hole, ball, arrow, datetime) : GameState = 
    printfn "Starting to animate"
    let x = Shapes.GetLeft(ball)
    let y = Shapes.GetTop(ball)
    printfn "%A" y
    let deltaTime = float (Clock.ElapsedMilliseconds - datetime)
    let xlength = abs (arrow.Line.X2 - arrow.Line.X1)
    let ylength = abs (arrow.Line.Y2 - arrow.Line.Y1)
    let length = Math.Sqrt((xlength * xlength) + (ylength * ylength))
    let vx = (arrow.Line.X2 - arrow.Line.X1) / length
    let vy = (arrow.Line.Y2 - arrow.Line.Y1) / length
    let xnew = x + (vx * 150.) * deltaTime / 1000.
    let ynew = y + (vy * 150.) * deltaTime / 1000.
    System.Diagnostics.Debug.WriteLine(sprintf "x:%A y:%A" xnew ynew)
    Shapes.Move(ball, xnew, ynew)
    BallFire(hole, ball, arrow, int(Clock.ElapsedMilliseconds))

let mutable gameState = Uninitialised

// Events
let OnKeyDown() = 
    let key = GraphicsWindow.LastKey
    printfn "%A" key
    match gameState with
    | Started(hole, ball, arrow) -> 
        match key with
        | "Space" -> gameState <- BallFire(hole, ball, arrow, Clock.ElapsedMilliseconds)
        | "Left" -> gameState <- ArrowMove(hole, ball, arrow, Left)
        | "Right" -> gameState <- ArrowMove(hole, ball, arrow, Right)
        | _ -> ()
    | _ -> ()

GraphicsWindow.KeyDown <- Callback(OnKeyDown)

let endGame ball = 
    let x = Shapes.GetLeft(ball)
    let y = Shapes.GetTop(ball)
    match x, y with
    | x, _ when x < 0. || x > (float GraphicsWindow.Width) -> true
    | _, y when y < 0. || y > (float GraphicsWindow.Height) -> true
    | _ -> false

let mutable started = false
let mutable counter = 0
// Main loop
GraphicsWindow.Update <- fun () -> 
    counter <- counter + 1
    match gameState with
    | Uninitialised -> 
        System.Diagnostics.Debug.WriteLine(sprintf "%A %A %d" Clock.ElapsedMilliseconds gameState counter)
        gameState <- initGame()
    | Started(hole, ball, arrow) ->
        if not started then
            started <- true 
            System.Diagnostics.Debug.WriteLine("Started")
    | ArrowMove(hole, ball, arrow, Left) -> 
        System.Diagnostics.Debug.WriteLine("ArrowMove Left")
        let arrow = arrow.MoveLeft ()
        gameState <- Started(hole, ball, arrow)
    | ArrowMove(hole, ball, arrow, Right) -> 
        System.Diagnostics.Debug.WriteLine("ArrowMove Right")
        let arrow = arrow.MoveRight ()
        gameState <- Started(hole, ball, arrow)
    | BallFire(hole, ball, arrow, lastTime) ->
        System.Diagnostics.Debug.WriteLine("BallFire")
        if endGame ball then gameState <- End
        else 
            arrow.Hide()
            gameState <- AnimateBallMove(hole, ball, arrow, lastTime)
    | End -> gameState <- initGame()
    | _ -> ()
