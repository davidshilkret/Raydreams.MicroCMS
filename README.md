# MicroCMS

A rediculously simple CMS that serves Markdown content files in either an Azure File Share or Blob Container.

No web server needed... no database is used.

This runs as a .NET 6 isolated Azure Function, which means it has a main function that you can inject into at startup just like a web API.

It uses the [MarkDig](https://github.com/xoofx/markdig) Markdown to HTML converter but you can easily uses any converter you like. All the Advanced Parsing options are turned on so you can add IDs, Classes and even custom attributes to the HTML from the Markdown. This allows you to tweak the layout from the CSS while still using Markdown.

## Example

[My Test Blog](https://blog.raydreams.com/page/index)

## Why

I really just wanted a simple way to post Markdown files without having to use someone else's service. Even [Jekyll](https://jekyllrb.com/) was overkill.

Markdown is much easier and faster to type without all the rediculous HTML.

## Endpoints

For now there are only a couple of endpoints :

* **GetPage** - Intercepts any call to the root for now and redirects back to the home page
  * path: [GET] /
* **GetPage** - Gets a single Markdown page from the data store and returns it as HTML or wrapped JSON
  * path: [GET] /page/<filename_with_no_ext>?layout=<file>&wrapped=<bool>
  * params:
    * layout (string) [optional] to use an explicit layout instead of the default one
    * wrapped (bool) [optional] set to true to return the page as wrapped JSON for using as headless
  * output: Either the HTML or a wrapped JSON object
* **GetImage** - Gets a single image of type JPEG, PNG, GIF, or ICO
  * path: [GET] /image/<filename_with_ext>
  * params: none
  * output: an image file
* **Ping** - Just test the service is working
  * path: [GET] /ping/<some_string_message_to_echo>
  * params: none
  * output: a JSON string
* **List** - Get an HTML list of all the pages. An integer of some value is required but has no effect just yet
  * path: [GET] /list/<int>
  * params: none
  * output: HTML list of files

## Getting Started

(More Details Incoming)

* Create an Azure Account with a Free Pay-As-You-Go subscription.
* Create a new Azure Function Resource. You can use a Windows or Liunx App Service.
    * You can use the Data Storage account created with it but I suggest giving it a better name like `blogdevsa01`
* Fork this repo and publish the project to our new Azure Function
    * You can publish from Visual Studio or just accept the default YML script from Deployment Center
    * Make sure you are using Azure Function runtime v4 (this is NOT the same as the .NET version)
* Upload some Markdown files, a base layout file and some images to the corresponding Data Store that was created with the Azure Function.
* Add a Single table called `Logs` to your Azure Storage Tables.

## Configure the function settings

There are handful of settings you will want to set either in `local.settings.json` or the Azure Function Configuration:

```
"Values": {
    "AzureWebJobsStorage": "<Already set in Azure Function>",
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

