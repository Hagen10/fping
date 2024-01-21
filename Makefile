# .PHONY: build
# build:
# 	docker build -f docker/osx.Dockerfile -t socket .

# .PHONY: run
# run: build
# 	docker run -d -p 10000:10000/udp --name socket socket

# .PHONY: stop
# stop:
# 	docker stop socket
# 	docker rm socket

# .PHONY: run-local
# run-local:
# 	dotnet run 10001

# .PHONY: run-local-recv
# run-local-recv:
# 	dotnet run recv

.PHONY: build-pi
build-pi:
	docker build --progress=plain -f docker/pi.Dockerfile . -t fping-pi