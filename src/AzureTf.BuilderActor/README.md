# Nothing special here
# Copied from https://github.com/dapr/dotnet-sdk/tree/master/samples/Actor

dapr run --port 3500 --app-id job-actor --app-port 5000 dotnet run

http://127.0.0.1:3500/v1.0/actors/job/abc/method/execute