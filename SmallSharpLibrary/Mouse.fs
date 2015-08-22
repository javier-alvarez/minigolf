namespace Library

[<Sealed>]
type Mouse private () =
   static member IsLeftButtonDown = My.App.IsLeftButtonDown
   static member IsRightButtonDown = My.App.IsRightButtonDown
   static member X = fst My.App.MousePosition
   static member Y = snd My.App.MousePosition
   static member HideCursor () =
      ()
   static member ShowCursor () =
      ()