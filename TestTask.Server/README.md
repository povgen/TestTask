# TestTask.Server
## Getting Started
### 1) RabbitMQ
for work you should to install 
[RabbitMQ](https://www.rabbitmq.com/download.html)

easiest way it's to use Docker image:
```sh
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
```
### 2) Project configuration
Need set some settings in appsettings.json
- MaxFilesSize - max size of all sent files (their sum) per request in bytes, 
it's optional values, if not set, will be use default value - 10 Mb
- StoragePath - path to storage for files (html and pdf), it's required
- RabbitMQHost - path to RabbitMQ server, it's required
- RabbitMQPort - port for RabbitMQ server, if not set will be use default from library (5672)

### 3) Launch server
