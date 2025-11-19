# FlixNet Project

## Description

This is a school project to demonstrate the use of Restful API's in a .NET Core project. 
Futhermore to get the hang of how to containerize with Docker Compose YAML IaC with the use of MongoDB and the webAPI.
We were meant to implement Authentication to our endpoints, but as time progressed we didn't have time to get this done.

## Getting Started

### Dependencies

No dependencies should be needed, as everything is run within Docker. The only must-have is to be run on a Windows computer.
This project uses:
- MongoDB 7.0
- Docker
- Docker Compose YAML IaC
- MongoDB.Driver ver. 3.5.0
- Microsoft.AspNetCore.OpenApi ver. 9.0.11

### Installing and Executing the program

1. The code in the Program.cs, lines 30-34, will seed the database with 3 movies, if you want to run the program again we recomend that you outcomments those lines to not have dublicates of the movies.

2. Open Docker Desktop(windows) or docker and run the following commands
    - open cmd and navigate to files.
    - docker compose build --no-cache
    - docker compose up

3. In your browser open this URL: http://localhost:8080/staticfiles/

4. Select a movie in the dropdown menu and press Watch movie

5. Congratulations the movie is now streaming!

## Authors
Allan Holm Iversen & Louise Sun Møller Leed

## Version
1.0
