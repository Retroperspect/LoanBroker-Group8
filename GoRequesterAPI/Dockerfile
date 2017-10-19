FROM golang:jessie

# Install any needed dependencies...
# RUN go get ...
RUN go get github.com/gorilla/mux



# Copy the server code into the container
COPY Handlers.go Handlers.go
COPY Logger.go Logger.go
COPY Main.go Main.go
COPY Router.go Router.go
COPY Routes.go Routes.go
COPY LongOP.go LongOP.go
COPY RequestResponse.go RequestResponse.go

# Make port 8989 available to the host
EXPOSE 8989

ENTRYPOINT go run Main.go Logger.go Handlers.go Router.go Routes.go LongOP.go RequestResponse.go