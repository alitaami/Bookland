Hello everyone

Recently me and my classmates have done a project for our university project, and we used Distributed Monolith architecture in the backend coding of this project.
We have different services in the project, which are written with different backend-coded frameworks, connecting into one shared database of Postgres SQL.

In the diagram below, you can see the general architecture and different back-end services and how they relate to each other :
![Services Diagram](https://github.com/alitaami/Bookland-MicroService/assets/116227297/20daea10-077f-43df-bbfb-b7f34b4d3679)

As you see, I have developed Wallet service and Order-Discount service for this OnlineBookStore project.

Wallet service : It is about charging wallets of customer and the publisher; I have used ZarinPalDemo Api for implementing this future. 
Order-Discount : It is for purchasing books by users and calculating the invoice if they used any discount or if they did not used.

You can build and run to test endpoints with this docker command in the path of the project : 

<h6>docker-compose -d --build</h6>

