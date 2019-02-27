<h1>CDON Email Sender</h1>

<p>EmailSender is a ficticious application that sends mail to customers using different mail types.</p>

<h2>Prerequisites</h2>
<ul>
    <li>Visual Studio 2017</li>
    <li>ASP.NET Core 2.2</li>
    <li>Visual Studio Code 1.31.1</li>
</ul>

<h3>Steps to run</h3>
<ul>
    <li>Build Solution.</li>
    <li>Set EmailSender.Console project as Startup project.</li>
    <li>Start with or without debugging with IIS Express on Visual Studio.</li>
</ul>

<h2>Solution</h2>

This solution consists of the following projects:

<ul>

<li><strong>EmailSender.BusinessLogic</strong></li>
Contains Service that implements business logic for email sending.</br></br>

<li><strong>EmailSender.BusinessLogic.Tests</strong></li>
Contains Tests for business logic.</br></br>

<li><strong>EmailSender.Console</strong></li>
Contains simple console app which implements business logic.</br></br>

<li><strong>EmailSender.DataLayer</strong></li>
Contains the Repository that holds temporary emailsender data. Can be extended to connect to real time database.</br></br>

<h2>Setup</h2>
<p>NodeJS must be installed on your system if you are planning to use VS code.</p>

<p>Clone or download this repository to your local machine. Then click the file EmailSender.sln file, this should open the solution correctly whereby you will be presented with an N-layer solution.

Ensure you set the Startup project to EmailSender.Console. Restore all Nuget and npm packages, then, right click the solution icon and select BUILD.</p>

<h2>Assumptions</h2>
<ul>
    <li>Default products and orders are used in DataLayer as per specifications.</li>
    <li>Default mail types are used in MailServer as per specification.</li>
    <li>Resource file is used to define the email templates.</li>
</ul>

<h2>Shortcomings in current code</h2>
<p>
1. You are filtering Customers and Orders in C# ; but as your business will be growing up and you get more and more customers and orders, you'll encounter performance issues.

This sould be done in SQL or whatever data technology you'd like to use. Maybe there's two ways to tackle this issue :

- either you change your DataLayer to expose methods like GetNewCustomers or GetCustomersWithoutRecentOrders which then will be calling Stored Procedure ;

- or, you can use an ORM like Entity Framework (or Nhibernate, there's a lot of frameworks like this) to expose IQueryable<T> return types in your DataLayer. Most of ORMs are able to translate C# linq queries (ie. expressions like customers.Where(x => somecondition)) to SQL queries.

- The datalayer methods should not be static as you want to ideally switch between implementations without changing the model code. Same with ListOrders. Here we can use a base abstract class or an interface.

- Classes Customer, Order and DataLayer should be sealed.

To my opinion, this is definitly a must fix if you need to move to production with this code.

2. I have written some code to handle failure in tour SmtpMailSender class. That's a ok for temporary but consider using a dedicated framework for this, like Polly (https://github.com/App-vNext/Polly). It's popular and already tested code that will saves you lot of time and help your app to be more resilient.

3. You should keep track of the email that you're sending to your customers. In mean, if you're operating in Europe, recent regulation like GDPR needs you to be exactly aware about the communication you've sent to your customer. You should also prove link in your email template for your customer to be able to opt ot (especially for your "comeback" email type).

Moreover, keeping tracks of email sent (or failed to sent) will help you to write a batch that will re-send the failed mails : let's say that your SMTP server is down for many hours, your front app will store in a table in your db each email that was not sent, and then you can start a batch console app that will try again to send these emails (this is really important for your customers to always receive the order confirmation emails).

</p>

<h2>Suggested Architecture</h2>
<p>
Always use interface over any internal or external service. See for example what i've done with Handlebar. By wrapping it up with an IMailTemplateRenderer interface, I've made the code testable and uncoupled from Handlebar. If tomorrow you decide to move away from Handlebare, fine, just write a new IMailTemplateRender implementation, run the unit tests and you're done.

You should consider using a DI container. This will help you in writing interfaces and wiring it with the proper implementation. .NET Core comes with a native DI container which is quite easy to use and will perform perfectly in your use case. Have a look at this for more information : https://stackify.com/net-core-dependency-injection/

To sums up, if I had to write a similar program, I will :

1. First, identify all services (internal and external) and design interfaces, lets say :

   - IDataLayer
   - IMailSender
   - IMailTemplateRenderer
   - IMailTemplateProvider (for example, if you want to get rid of the .resx file and use a database instead)

2. Write the implementations and use constructor dependency injection when one service needs to reference another one

3. Set up a DI container in my Console App, start it and watch it run :)

4. Moving further I would use BDD to define domain model for the system and use CQRS and Message Broker to design a effective distributed application.
   </p>
