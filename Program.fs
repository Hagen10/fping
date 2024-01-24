// For more information see https://aka.ms/fsharp-console-apps
open IcmpListener
open System

let main() =
    printfn "APPLICATION STARTED!!!!!!"

    let ip = 
        let tempIp = Environment.GetEnvironmentVariable("PING_IP")

        if tempIp = null then "8.8.8.8" else tempIp

    printfn "FOUND ENVIRONMENT VARIABLE %s" ip

    let icmpListener = IcmpListener(ip)
    icmpListener.StartListening()

    // Keep the application running (Ctrl+C to stop)
    while true do
        Console.ReadLine() |> ignore        

    icmpListener.StopListening()

main()