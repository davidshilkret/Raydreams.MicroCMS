# MicroCMS

A rediculously simple CMS that serves Markdown content files in either an Azure File Share or Blob Container.

No web server needed... no database is used.

This runs as a .NET 6 isolated Azure Function, which means it has a main function that you can inject into at startup just like a web API.

## Example

[My Test Blog](https://blog.raydreams.com/page/index)

## Getting Started

(More Details Incoming)

* Create an Azure Account with a Free Pay-As-You-Go subscription.
* Create a new Azure Function Resource. You can use a Windows or Liunx App Service.
    * You can use the Data Storage account created with it but I suggest giving it a better name like `blogdevsa01`
* Fork this repo and publish the project to our new Azure Function
    * You can publish from Visual Studio or just accept the default YML script from Deployment Center
    * Make sure you are using Azure Function runtime v4 (this is NOT the same as the .NET version)
* Upload some Markdown files, a base layout file and some images to the corresponding Data Store that was created with the Azure Function

## Configure the function settings

There are handful of settings you will want to set:

```
"Values": {
    "AzureWebJobsStorage": "",
    "ASPNETCORE_ENVIRONMENT": "Development",
    "connStr": "Connection String to the Data Storage You want to use",
    "root": "blog",
    "homepage": "index",
    "errorpage": "error",
    "imageDir": "images",
    "layoutDir": "layouts",
    "layoutExt": "html",
    "store": "file"
}
```

## Testing

No unit test yet just simple integration test. More to come.

