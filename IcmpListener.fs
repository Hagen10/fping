// IcmpListener.fs

module IcmpListener

open System
open System.Net.Sockets
open System.Threading
open System.Net
open System.Net.NetworkInformation

type Message =
    | Ping
    | Pong
    | Error

type IcmpListener(ip : string) =
    let destIp = ip

    let ping = new Ping()

    let bufferSize = 1024
    let icmpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp)

    member this.pingIp() : IPStatus =
        let ipAddr = IPAddress.Parse(destIp)

        let reply = ping.Send(ipAddr)

        reply.Status

    member this.listenForPings() =
        try
            let buffer = Array.zeroCreate<byte> bufferSize
            let endpoint = ref (IPEndPoint(IPAddress.Any, 0) :> EndPoint)
            endpoint := new IPEndPoint(IPAddress.Any, 0)

            // let endPoint = new IPEndPoint(IPAddress.Any, 0)

            icmpSocket.Bind(endpoint.Value)
            // icmpSocket.IOControl((IOControlCode.ReceiveAll : IOControlCode), ([| 1uy; 0uy; 0uy; 0uy |] : byte[]), (null : byte[])) |> ignore

            // icmpSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0)

            while true do
                // let endpoint = ref (IPEndPoint(IPAddress.Any, 0) :> EndPoint)

                let receivedBytes = icmpSocket.ReceiveFrom(buffer, endpoint)
                // Process the received ICMP packet, parse data, etc.
                printfn "Received ICMP packet with %d bytes" receivedBytes

                // Extract the source IP address from endPoint
                let sourceIPAddress = match endpoint.Value with
                                        | :? IPEndPoint as ep -> ep.Address.ToString()
                                        | _ -> "Unknown"

                // Not sure if this check is even necessary
                match sourceIPAddress = destIp, this.getMessageType buffer with
                | false, Ping ->  match this.pingIp() with
                                    | IPStatus.Success -> 
                                        printfn "Received pong from %s, sending icmp reply to %s" destIp sourceIPAddress
                                        let replyBytes = this.createIcmpEchoReply buffer

                                        // icmpSocket.SendTo(replyBytes, endpoint.Value) |> ignore

                                        icmpSocket.SendTo(replyBytes, 0, replyBytes.Length, SocketFlags.None, endpoint.Value) |> ignore

                                    | _ -> printfn "COULDN'T PING %s" destIp
                | _, msg -> printfn "Received a ping from the destination ip %s - %A" destIp msg



        with
        | :? ObjectDisposedException -> () // Socket was disposed, exit thread

    member this.StartListening() =
        let listenerThread = new Thread(fun () -> this.listenForPings())
        listenerThread.Start()

    member this.StopListening() =
        icmpSocket.Close()

    member private this.createIcmpEchoReply (requestBuffer : byte []) =
        // Copy the entire request packet for the reply
        let replyBuffer = Array.copy requestBuffer

        // Set the type field to ICMP Echo Reply (0)
        replyBuffer.[20] <- 0uy

        // Set the checksum field to 0 for now
        replyBuffer.[22] <- 0uy
        replyBuffer.[23] <- 0uy

        // Calculate and set the new checksum
        let rec calculateChecksum (startIndex: int) (length: int) =
            let mutable sum = 0
            let mutable i = startIndex

            while i < (startIndex + length) do
                sum <- sum + int replyBuffer.[i]
                i <- i + 2

            while (sum >>> 16) <> 0 do
                sum <- (sum &&& 0xFFFF) + (sum >>> 16)

            sum

        let checksum = calculateChecksum 20 (Array.length replyBuffer - 20)
        replyBuffer.[22] <- byte checksum
        replyBuffer.[23] <- byte (checksum >>> 8)

        replyBuffer

    member private this.getMessageType (bytes : byte []) : Message =
        match bytes.[20] with
        | 8uy -> Ping
        | 0uy -> Pong
        | _ -> Error
