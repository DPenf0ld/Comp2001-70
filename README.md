# Comp2001-70 API Documentation

## Project information
The task is to create a robust micro-service communicates with my SQL database. Additionally, an authenticator API must be used to verify admin accounts.

## Controllers:

### Login:
User must enter their email and password. Upon logging in, the user will be met with 1 of 3 responses. 
- First response is if their credentials are incorrect.
- Second response is if they've logged in successfully as a user.
- Third response is if they log in successfully as an admin.

### Admin Get:
This allows an admin to get all information of every profile. If the logged-in account is not an admin, they will get a response saying "Account does not have permissions." If the user is not logged in, they will get the response "Please login to an admin account."

### Admin Delete:
This allows an admin to archive an account based on a userID. If the logged-in account is not an admin, they will get a response saying "Account does not have permissions." If the user is not logged in, they will get the response "Please login to an admin account." Additionally, if the userId is not in the SQL database, they will get the response "User not found."

### UserInfo Get:
Gets all information of the logged-in user based on their userID (The user does not have to enter their userID; this is stored when they log in). If the user is not logged in, they will get a response prompting them to login. 

### UserInfo Post:
Anyone can use this; the user must enter the details of the account they want to create. Once created, the response will inform the user that the account has been created. If the information is invalid, they will get a response telling them the account could not be created.

### UserInfo Put:
Once logged in, users can enter the column name they want to update with the updated value. Once updated, a response will confirm this. If the column name or value is invalid, the response will tell them information could not be updated. (The user does not have to enter their userID; this is stored when they log in).

### UserInfo Delete:
Users who are logged in can archive their account. (The user does not have to enter their userID; this is stored when they log in). If the user is not logged in, they will be prompted to do so in the response.

### ViewProfiles Get:
Anyone can use this feature. Non-sensitive information about all accounts in the SQL database is displayed.
