open Library

let onKeyDown () =   
   match GraphicsWindow.LastKey with
   | "D1" -> GraphicsWindow.PenColor <- Colors.Red
   | "D2" -> GraphicsWindow.PenColor <- Colors.Blue
   | "D3" -> GraphicsWindow.PenColor <- Colors.LightGreen
   | "C"  -> GraphicsWindow.Clear()
   | _    -> ()

let mutable prevX = 0.0
let mutable prevY = 0.0

let onMouseDown () =
   prevX <- GraphicsWindow.MouseX
   prevY <- GraphicsWindow.MouseY
   
let onMouseMove () =
   let x = GraphicsWindow.MouseX
   let y = GraphicsWindow.MouseY
   if Mouse.IsLeftButtonDown then
      GraphicsWindow.DrawLine(prevX, prevY, x, y)
   prevX <- x
   prevY <- y

GraphicsWindow.BackgroundColor <- Colors.Black
GraphicsWindow.PenColor <- Colors.White
GraphicsWindow.MouseDown <- Callback(onMouseDown)
GraphicsWindow.MouseMove <- Callback(onMouseMove)
GraphicsWindow.KeyDown <- Callback(onKeyDown)
