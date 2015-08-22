module internal Library.Http

open System.IO
open System.Net

type Stream with
    member inputStream.CopyTo(outputStream:Stream) =
        let readSize = 2048
        let buffer = Array.zeroCreate readSize
        let mutable count = inputStream.Read(buffer,0,readSize)
        while count > 0 do
            outputStream.Write(buffer,0, count)
            count <- inputStream.Read(buffer,0,readSize)        

let LoadBytes (url:string) =
   let request = HttpWebRequest.Create(url)
   let response = request.GetResponse()  
   use responseStream = response.GetResponseStream()     
   use memoryStream = new MemoryStream()  
   responseStream.CopyTo(memoryStream)
   memoryStream.GetBuffer()