﻿I decided to detach processing html files to separate the service.
Because, it's the most loaded part of the application.

For connecting this service with the web-server, I used rabbitMQ.

It's making service scalable, because you can use a few instances of handler for file processing.

Also, it's allow to get stable web service,
because if one of part system will be reloaded state of the file will be saved.

P.S.: 
- For working this services on separated machine, need to use NFS. 
In future (if it's necessary) it could be replaced
on cloud file system (like Google or Azure) 
- Also need to add Store Cleaning on disk-hosting side or app side