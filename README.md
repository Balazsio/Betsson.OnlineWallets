# qa-backend-code-challenge

Code challenge for QA Backend Engineer candidates.

### Build Docker image

Run this command from the directory where there is the solution file.

```
docker build -f src/Betsson.OnlineWallets.Web/Dockerfile .
```

### Run Docker container

```
docker run -p <port>:8080 <image id>


example:
docker run -p 8080:8080 sha256:9ca7a1bfc35eb4002d6401d749366a490840aaf7efd4969d7b0e2112c97c71b2
```

### Open Swagger

```
http://localhost:<port>/swagger/index.html

example:
http://localhost:8080/swagger/index.html
```
