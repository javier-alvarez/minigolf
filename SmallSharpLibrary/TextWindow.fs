namespace Library

open System

[<Sealed()>]
type TextWindow private () =
    static member Write(number:int) = Console.Write number
    static member Write(text:string) = Console.Write text
    static member Write(value:obj) = Console.Write value
    static member WriteLine(number:int) = Console.WriteLine number
    static member WriteLine(text:string) = Console.WriteLine text
    static member WriteLine(value:obj) = Console.WriteLine value
    static member Read() = Console.Read()
    static member ReadLine() = Console.ReadLine()
    