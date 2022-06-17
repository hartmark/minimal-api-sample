# Introduction 
Simple demo service that uses minimal API. I felt that all demos I found online was a bit lacking on the real-world
usage of minimal API and how to structure it.

The application is meant to showcase how to setup a simple service using minimal API.

It uses JWT for authenticating the calls and FluentValidation for validating requests.

All operations are tested with xunit-tests using testserver.

The main reason of why to post this repo was to have a more complete solution that showcases how to successfully use
minimal API in your solution.

# Structure
The service showcases a simple "login" endpoint to get a valid JWT. The users are for now hardcoded in the 
UserRepository class.

Note that the keys for the signing the JWT is generated on startup so old tokens from an old run won't work.

There are two roles and the user "harre" has write permissions so all operations that does writing, and "noob" has
only read permissions.

The service persist the data just in memory and is gone when the server restarts.

