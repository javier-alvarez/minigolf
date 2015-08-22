namespace Library 

open System
open System.Threading        
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Input
open System.Windows.Media.Imaging

[<Sealed>]
type GraphicsWindow private () =
    static let mutable title = ""
    static let mutable topMost = false
    static let mutable backgroundColor = Colors.White
    static let mutable penColor = Colors.Black
    static let mutable penWidth = 1.0
    static let mutable brushColor = Colors.Blue
    static let mutable opacity = 1.0
    static let mutable fontSize = 12.0
    static let mutable fontName = ""
    static let mutable fontBold = false
    static let mutable fontItalic = false
    static let initStroke (shape:#Shape, color, thickness) =
        shape.Opacity <- opacity        
        shape.Stroke <- Brush.From color
        shape.StrokeThickness <- thickness
    static let initFill (shape:#Shape, color) =
        shape.Opacity <- opacity 
        shape.Fill <- Brush.From color
    static let dispatch f = 
        My.App.Dispatcher.BeginInvoke(Action(fun () -> f My.App.Window)) |> ignore    
    static let invokeWithReturn (f:Window -> 'a) = 
        My.App.Dispatcher.Invoke(System.Func<_>(fun () -> f My.App.Window)) :?> 'a
    static let draw f =
        My.App.Dispatcher.BeginInvoke(Action(fun () -> f My.App.Canvas)) |> ignore
    static member Update with set f = My.App.Update <- f
    static member Topmost
        with get() = topMost
        and set(value) =
            topMost <- value
            dispatch (fun window -> window.Topmost <- value)
    static member Title
        with get() = title            
        and set(value) =
            title <- value
            dispatch (fun window -> window.Title <- value)
    static member Width
        with get() = 
            invokeWithReturn (fun window -> (window.Content :?> Canvas).ActualWidth |> int)
        and set(width) = 
            dispatch (fun window -> (window.Content :?> Canvas).Width <- float width)
    static member Height
        with get() = 
            invokeWithReturn (fun window -> (window.Content :?> Canvas).ActualHeight |> int)
        and set(height) = 
            dispatch (fun window -> (window.Content :?> Canvas).Height <- float height)
    static member BackgroundColor
        with get() = backgroundColor
        and set(color) =
            backgroundColor <- color
            dispatch (fun window -> window.Background <- Brush.From color)
    static member PenColor
        with get() = penColor
        and set(color) = penColor <- color
    static member PenWidth
        with get() = penWidth
        and set(width) = penWidth <- width
    static member BrushColor
        with get() = brushColor
        and set(color) = brushColor <- color
    static member Opacity
        with get() = opacity
        and set(value) = opacity <- value
    static member GetColorFromRGB(r,g,b) = Color(255uy,byte r,byte g,byte b)
    static member FontSize 
        with get () = fontSize
        and set(value) = fontSize <- value
    static member FontName 
        with get () = fontName
        and set (value) = fontName <- value
    static member FontBold
        with get () = fontBold
        and set (value) = fontBold <- value
    static member FontItalic 
        with get () = fontItalic
        and set (value) = fontItalic <- value
    static member Show() =
        dispatch (fun window -> 
            if not window.IsVisible then
                window.Show()
        )
    static member Hide() =
        dispatch (fun window -> window.Hide())
    static member Clear() =
        draw (fun canvas -> 
            canvas.Children.Clear()
            canvas.Children.Add(My.App.Turtle) |> ignore
        )
    static member DrawLine(x1:float,y1:float,x2:float,y2:float) =
        let color, thickness = penColor, penWidth
        draw (fun canvas ->
            let line = Line(X1=x1,Y1=y1,X2=x2,Y2=y2)
            initStroke(line,color,thickness)
            line |> canvas.Children.Add |> ignore
        )
    static member DrawLine(x1:int,y1:int,x2:int,y2:int) =
        GraphicsWindow.DrawLine(float x1, float y1, float x2, float y2)
    static member DrawPolygon(points) =
        let color, thickness = penColor, penWidth
        draw (fun canvas ->
            let points = [|for (x,y) in points -> Point(x,y)|]
            let polygon = Polygon(Points=System.Windows.Media.PointCollection(points))            
            initStroke(polygon,color,thickness)
            polygon |> canvas.Children.Add |> ignore
        )
    static member FillPolygon(points) =
        let color = brushColor
        draw (fun canvas ->
            let points = [|for (x,y) in points -> Point(x,y)|]
            let polygon = Polygon(Points=System.Windows.Media.PointCollection(points))            
            initFill(polygon,color)
            polygon |> canvas.Children.Add |> ignore
        )
    static member DrawEllipse(x:float,y:float,width:float,height:float) =
        let color, thickness = penColor, penWidth
        draw (fun canvas ->
            let ellipse = Ellipse(Width=width,Height=height)
            initStroke(ellipse,color,thickness)
            Canvas.SetLeft(ellipse,x)
            Canvas.SetTop(ellipse,y)
            ellipse |> canvas.Children.Add |> ignore
        )
    static member DrawEllipse(x:int,y:int,width:int,height:int) =
        GraphicsWindow.DrawEllipse(float x, float y, float width, float height)
    static member FillEllipse(x:float,y:float,width:float,height:float) =
        let color = brushColor
        draw (fun canvas ->
            let ellipse = Ellipse(Width=width,Height=height)
            initFill(ellipse,color)
            Canvas.SetLeft(ellipse,x)
            Canvas.SetTop(ellipse,y)
            ellipse |> canvas.Children.Add |> ignore
        )
    static member FillEllipse(x:int,y:int,width:int,height:int) =
        GraphicsWindow.FillEllipse(float x, float y, float width, float height)
    static member DrawRectangle(x:float,y:float,width:float,height:float) =
        let color, thickness = penColor, penWidth
        draw (fun canvas ->
            let rectangle = Rectangle(Width=width,Height=height)
            initStroke(rectangle,color,thickness)
            Canvas.SetLeft(rectangle,x)
            Canvas.SetTop(rectangle,y)
            rectangle |> canvas.Children.Add |> ignore
        )
    static member DrawRectangle(x:int,y:int,width:int,height:int) =
        GraphicsWindow.DrawRectangle(float x, float y, float width, float height)
    static member FillRectangle(x:float,y:float,width:float,height:float) =
        let color = brushColor
        draw (fun canvas ->
            let rectangle = Rectangle(Width=width,Height=height)
            initFill(rectangle,color)
            Canvas.SetLeft(rectangle,x)
            Canvas.SetTop(rectangle,y)
            rectangle |> canvas.Children.Add |> ignore
        )
    static member FillRectangle(x:int,y:int,width:int,height:int) =
        GraphicsWindow.FillRectangle(float x, float y, float width, float height)
    static member DrawImage(imageName,x,y) =
        draw (fun canvas ->
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
            canvas.Children.Add(image) |> ignore
        )
    static member DrawImage(imageName,x:int,y:int) =
        GraphicsWindow.DrawImage(imageName,float x,float y)
    static member DrawText(x:float,y:float,text:string) =
        let fontSize, fontName = GraphicsWindow.FontSize, GraphicsWindow.FontName
        let fontItalic, fontBold = GraphicsWindow.FontItalic, GraphicsWindow.FontBold
        draw (fun canvas ->
            let block = TextBlock(Text=text)
            block.FontSize <- fontSize
            block.FontFamily <- Media.FontFamily(fontName)
            if fontItalic then block.FontStyle <- FontStyles.Italic
            if fontBold then block.FontWeight <- FontWeights.Bold
            Canvas.SetLeft(block,x)
            Canvas.SetTop(block,y)
            block |> canvas.Children.Add |> ignore
        )    
    static member DrawText(x:int,y:int,text:string) =
        GraphicsWindow.DrawText(float x,float y,text)
    static member MouseDown with set handler = My.App.MouseDown <- handler
    static member MouseUp with set handler = My.App.MouseUp <- handler
    static member MouseMove with set handler = My.App.MouseMove <- handler
    static member MouseX with get () = fst My.App.MousePosition
    static member MouseY with get () = snd My.App.MousePosition
    static member KeyDown with set handler = My.App.KeyDown <- handler
    static member KeyUp with set handler = My.App.KeyUp <- handler
    static member LastKey with get () = My.App.LastKey
    static member ShowMessage(text,title) = ()