# .PHONY: build
# build:
# 	docker build -f docker/osx.Dockerfile -t socket .

# .PHONY: run
# run: build
# 	docker run -d -p 10000:10000/udp --name socket socket

.PHONY: stop-osx
stop-osx:
	docker stop fping-osx
	docker rm fping-osx

# .PHONY: run-local
# run-local:
# 	dotnet run 10001

# .PHONY: run-local-recv
# run-local-recv:
# 	dotnet run recv

.PHONY: build-pi
build-pi:
	docker build --progress=plain -f docker/pi.Dockerfile . -t fping-pi

.PHONY: build-osx
build-osx:
	docker build --progress=plain -f docker/osx-arm.Dockerfile . -t fping-osx

.PHONY: run-osx
run-osx: build-osx
	docker run -d --privileged -e "PING_IP=4.4.4.4" --name fping-osx fping-osx
	docker exec -u root fping-osx sh -c "sysctl -w net.ipv4.icmp_echo_ignore_all=1"