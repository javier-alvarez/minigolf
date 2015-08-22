namespace Library

open System
open System.Collections.Generic
open System.Windows
open System.Windows.Shapes
open System.Windows.Controls
open System.Windows.Media.Imaging

type internal Shape () = 
    let mutable element : UIElement = null
    let mutable x = 0.0
    let mutable y = 0.0
    let mutable opacity = 0
    member __.Element 
        with get () = element
        and set value = element <- value
    member __.X
        with get () = x
        and set value = x <- value
    member __.Y
        with get () = y
        and set value = y <- value
    member __.Opacity
        with get () = opacity
        and set value = opacity <- value

[<Sealed>]
type Shapes private () =
    static let shapes = Dictionary<string,Shape>()
    static let lineNo = ref 0
    static let ellipseNo = ref 0
    static let rectangleNo = ref 0
    static let triangleNo = ref 0
    static let imageNo = ref 0
    static let textNo = ref 0
    static let dispatch f = My.App.Dispatcher.BeginInvoke(Action(f)) |> ignore
    static member Remove(shapeName) =
        match shapes.TryGetValue(shapeName) with
        | true, shape -> 
            dispatch (fun () -> 
                shapes.Remove(shapeName) |> ignore
                My.App.Canvas.Children.Remove(shape.Element)
            )
        | false, _ -> ()
    static member ShowShape(shapeName) =
        match shapes.TryGetValue(shapeName) with
        | true, shape -> dispatch (fun () -> shape.Element.Visibility <- Visibility.Visible)
        | false, _ -> ()
    static member HideShape(shapeName) =
        match shapes.TryGetValue(shapeName) with
        | true, shape -> dispatch (fun () -> shape.Element.Visibility <- Visibility.Collapsed)
        | false, _ -> ()
    static member GetOpacity(shapeName) =
        match shapes.TryGetValue(shapeName) with
        | true, shape -> shape.Opacity
        | false, _ -> 0
    static member SetOpacity(shapeName, level) =
        match shapes.TryGetValue(shapeName) with
        | true, shape ->
            shape.Opacity <- level
            dispatch (fun () ->
                shape.Element.Opacity <- float level / 100.0
            )
        | false, _ -> ()
    static member Move(shapeName,x,y) =
        match shapes.TryGetValue(shapeName) with
        | true, shape ->
            shape.X <- x
            shape.Y <- y
            dispatch (fun () ->
                Canvas.SetLeft(shape.Element, x)
                Canvas.SetTop(shape.Element, y)
            )
        | false, _ -> ()
    static member Move(shapeName,x:int,y:int) =
        Shapes.Move(shapeName,float x,float y)
    static member GetLeft(shapeName) =
        match shapes.TryGetValue(shapeName) with
        | true, shape -> shape.X
        | false, _ -> 0.0
    static member GetTop(shapeName) =
        match shapes.TryGetValue(shapeName) with
        | true, shape -> shape.Y
        | false, _ -> 0.0
    static member Rotate(shapeName,angle) =
        match shapes.TryGetValue(shapeName) with
        | true, shape -> 
            dispatch (fun () ->
                let transform =
                    match shape.Element.RenderTransform  with
                    | :? Media.RotateTransform as t -> t
                    | _ ->                     
                        let transform = Media.RotateTransform()
                        shape.Element.RenderTransform <- transform
                        transform
                let fe = shape.Element :?> FrameworkElement
                transform.CenterX <- fe.ActualWidth / 2.0
                transform.CenterY <- fe.ActualHeight / 2.0
                transform.Angle <- angle
            )
        | false, _ -> ()
    static member Rotate(shapeName,angle:int) =
        Shapes.Rotate(shapeName,float angle)
    static member AddLine (x1:float,y1:float,x2:float,y2:float) =
        incr lineNo
        let name = "Line"+(!lineNo).ToString()
        let shape = Shape()
        shapes.Add(name, shape)
        let color, thickness = GraphicsWindow.PenColor, GraphicsWindow.PenWidth
        dispatch (fun () ->
            let line = Line(X1=x1,Y1=y1,X2=x2,Y2=y2)
            line.Stroke <- Brush.From color
            line.StrokeThickness <- thickness
            shape.Element <- line
            My.App.Canvas.Children.Add(line) |> ignore
        )
        name
    static member AddLine (x1:int,y1:int,x2:int,y2:int) =
        Shapes.AddLine(float x1,float y1,float x2,float y2)
    static member AddEllipse (width:float,height:float) =
        incr ellipseNo
        let name = "Ellipse"+(!ellipseNo).ToString()
        let shape = Shape()
        shapes.Add(name, shape)
        let stroke, thickness = GraphicsWindow.PenColor, GraphicsWindow.PenWidth
        let fill = GraphicsWindow.BrushColor
        dispatch (fun () ->
            let ellipse = Ellipse(Width=width,Height=height)
            ellipse.Stroke <- Brush.From stroke
            ellipse.StrokeThickness <- thickness
            ellipse.Fill <- Brush.From fill           
            shape.Element <- ellipse
            My.App.Canvas.Children.Add(ellipse) |> ignore
        )
        name
    static member AddEllipse (width:int, height:int) =
        Shapes.AddEllipse(float width, float height)
    static member AddRectangle (width:float, height:float) =
        incr rectangleNo
        let name = "Rectangle"+(!rectangleNo).ToString()
        let shape = Shape()
        shapes.Add(name, shape)
        let stroke, thickness = GraphicsWindow.PenColor, GraphicsWindow.PenWidth
        let fill = GraphicsWindow.BrushColor
        dispatch (fun () ->
            let rectangle = Rectangle(Width=width,Height=height)            
            rectangle.Stroke <- Brush.From stroke
            rectangle.StrokeThickness <- thickness
            rectangle.Fill <- Brush.From fill
            shape.Element <- rectangle
            My.App.Canvas.Children.Add(rectangle) |> ignore           
        )
        name
    static member AddRectangle (width:int, height:int) =
        Shapes.AddRectangle(float width, float height)
    static member AddTriangle(x1,y1,x2,y2,x3,y3) =
        incr triangleNo
        let name = "Triangle"+(!triangleNo).ToString()
        let shape = Shape()
        shapes.Add(name, shape)
        let stroke, thickness = GraphicsWindow.PenColor, GraphicsWindow.PenWidth
        let fill = GraphicsWindow.BrushColor
        dispatch (fun () ->
            let poly = Polygon()
            poly.Points.Add(Point(x1, y1))
            poly.Points.Add(Point(x2, y2))
            poly.Points.Add(Point(x3, y3))
            poly.Stroke <- Brush.From stroke
            poly.StrokeThickness <- thickness
            poly.Fill <- Brush.From fill
            shape.Element <- poly
            My.App.Canvas.Children.Add(poly) |> ignore
        )
        name
    static member AddTriangle(x1:int,y1:int,x2:int,y2:int,x3:int,y3:int) =
        Shapes.AddTriangle(float x1,float y1,float x2,float y2,float x3,float y3)
    static member AddText(text:string) =
        incr textNo
        let name = "Text"+(!textNo).ToString()
        let shape = Shape()
        shapes.Add(name, shape)
        let foreground = GraphicsWindow.BrushColor
        let fontSize, fontName = GraphicsWindow.FontSize, GraphicsWindow.FontName
        let fontItalic, fontBold = GraphicsWindow.FontItalic, GraphicsWindow.FontBold
        dispatch (fun () ->
            let block = TextBlock(Text=text)
            block.Foreground <- Brush.From foreground
            block.FontSize <- fontSize
            block.FontFamily <- Media.FontFamily(fontName)
            if fontItalic then block.FontStyle <- FontStyles.Italic
            if fontBold then block.FontWeight <- FontWeights.Bold
            shape.Element <- block
            My.App.Canvas.Children.Add(block) |> ignore            
        )
        name
    static member SetText(shapeName:string,text:string) =
        match shapes.TryGetValue(shapeName) with
        | true, shape ->
            dispatch (fun () ->
                let block = shape.Element :?> TextBlock
                block.Text <- text
            )
        | false, _ -> ()
    static member AddImage(imageName:string) =
        incr imageNo
        let name = "Image"+(!imageNo).ToString()
        let shape = Shape()
        shapes.Add(name, shape)
        dispatch (fun () ->
            let bitmap = BitmapImage()
            bitmap.BeginInit()
            match ImageList.TryGetImageBytes(imageName) with
            | Some bytes ->                
                let stream = new System.IO.MemoryStream(bytes)                
                bitmap.StreamSource <- stream
            | None -> 
                bitmap.UriSource <- Uri(imageName, UriKind.Absolute)
            bitmap.EndInit()            
            let image = Image(Source=bitmap)
            shape.Element <- image
            My.App.Canvas.Children.Add(image) |> ignore
        )
        name

    