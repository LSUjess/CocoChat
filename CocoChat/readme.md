
# Assumptions 
#  1. you already have an AWS profile/account
#  2. Instance will be in the US East (N. Virgina) region -- us-east-1

#  This code was written in Visual Studio 2019 C# using AWS Toolkit for Visual Studio 
#  You need to install and update the toolkit within Visual Studio by using Tools ≫ Extensions ≫ Manage Extensions 
#  You will have to provide AWS profile credentials by adding a new profile inside
#  AWS Explorer or upload a credentials file
# https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/credentials.html

# In the project, right click on CocoChat >> Publish to Elastic Beanstalk.
# https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/deployment-beanstalk-traditional.html
# For region, select US East (N. Virgina).

# For example, this instance is the instance running my api
# Mine is http://eubanks2020.us-east-1.elasticbeanstalk.com/api/CocoChat/jess

# To run the API
# Request: url/api/chats/jess/this is a test 1241/9000
# Response: { "id": 1234  }

# Request: url/api/chats/:username
# Response: [{ "id": 1234, "text": "this is a test" }, { "id": 1235, "text": "this is a test also" }]

# Request: url/api/chats/:id
# Response: { "username": "jess", "text": "this is a test", "expiration_date": ""2020-02-18T18:30:01Z"" }
