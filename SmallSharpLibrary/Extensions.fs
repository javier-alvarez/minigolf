namespace Library

open System.Collections.Generic
open System.Runtime.CompilerServices

[<Sealed>]
[<Extension>]
type Extensions private () =
    [<Extension>]
    static member Times(count:int, action:System.Action<int>) =
        for i=1 to count do action.Invoke(i)
    [<Extension>]
    static member Each(xs:IEnumerable<'T>, action:System.Action<'T>) =
        for x in xs do action.Invoke(x)
    [<Extension>]
    static member ForAll(xs:IEnumerable<'T>, predicate:System.Predicate<'T>) =
        xs |> Seq.forall predicate.Invoke
    [<Extension>]
    static member Exists(xs:IEnumerable<'T>, predicate:System.Predicate<'T>) =
        xs |> Seq.exists predicate.Invoke
    [<Extension>]
    static member Filter(xs:IEnumerable<'T>, predicate:System.Predicate<'T>) =
        xs |> Seq.filter predicate.Invoke
    [<Extension>]
    static member Find(xs:IEnumerable<'T>, predicate:System.Predicate<'T>) =
        xs |> Seq.find predicate.Invoke
    [<Extension>]
    static member Map(xs:IEnumerable<'T>, map:System.Func<'T,'U>) =
        xs |> Seq.map map.Invoke
    [<Extension>]
    static member Reduce(xs:IEnumerable<'T>, reduce:System.Func<'T,'T,'T>) =
        xs |> Seq.reduce (fun acc x -> reduce.Invoke(acc,x))
    [<Extension>]
    static member SortBy(xs:IEnumerable<'T>, sort:System.Func<'T,'Key>) =
        xs |> Seq.sortBy sort.Invoke
