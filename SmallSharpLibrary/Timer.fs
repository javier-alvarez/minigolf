namespace Library

[<Sealed>]
type Timer private () =
   static let mutable onTimer = Callback(ignore)
   static let timer = new System.Timers.Timer()
   static do timer.Elapsed.Add(fun args -> onTimer.Invoke())
   static member Pause() = timer.Stop()
   static member Resume() = timer.Start()
   static member Tick 
       with set (callback:Callback) = 
           onTimer <- callback
           timer.Start()
   static member Interval 
       with set (ms:int) = timer.Interval <- float ms
