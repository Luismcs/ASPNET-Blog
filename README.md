# Gaming Blog

## Description
This project is a gaming blog developed in ASP.NET. The blog has two types of users: `User` and `Admin`. Users can create and edit their own posts, as well as rate posts from other users. Administrators have additional permissions to manage all content and users in the system.

## Features

### Users
- Create posts
- Edit their own posts
- Rate posts from other users (1 to 5)

### Admins
- Create posts (can choose public or restricted visibility)
- Edit and delete any post in the system
- Edit user profiles

### Google Authenticator
- Implemented Google Authenticator for additional security

## Technologies Used
- ASP.NET
- Google Authenticator
- SQL Server
- HTML/CSS/JavaScript

## Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/Luismcs/ASP.NET-Blog
    ```

2. Navigate to the project directory:
    ```bash
    cd WebApplication1
    ```

3. Restore the project dependencies:
    ```bash
    dotnet restore
    ```

4. Configure the database connection string in the `appsettings.json` file.

5. Apply migrations to create the database:
    ```bash
    dotnet ef database update
    ```

6. Run the project:
    ```bash
    dotnet run
    ```

## Usage

### Create Account
- Register using the sign-up feature.
- Use Google authentication to add an extra layer of security.

### User Roles

#### User
- Access the dashboard to create and edit your own posts.
- Rate posts from other users from 1 to 5 stars.

#### Admin
- Access the admin panel to manage all posts and users.
- Create posts and set their visibility (public or restricted).
- Edit or delete any post in the system.
- Edit user profiles as needed.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.


## Gallery
<div align="center">
  <img src="https://github.com/Luismcs/ASP.NET-Blog/blob/main/1-%20Home%20Page.png" alt="Home_Page">
</div>
<div align="center">
  <img src="https://github.com/Luismcs/ASP.NET-Blog/blob/main/2-%20Post%20List.png" alt="Posts_Index">
</div>
<div align="center">
  <img src="https://github.com/Luismcs/ASP.NET-Blog/blob/main/3-%20Post%20View.png" alt="Post_Edit">
</div>
