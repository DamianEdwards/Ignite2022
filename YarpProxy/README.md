# Simple proxy

You can use [YARP's](https://www.nuget.org/packages/Yarp.ReverseProxy) `IHttpForwarder` to handle all of the boilerplate logic to
proxy an incoming HTTP request to another backend. This handles HTTP (1, 2 and 3), websockets and gRPC proxying out of the box.
