Feature: Quiz Tracker Page

Background: 
Given I am an unauthenticated user
When I login with Azure AD
And I am on the "QuizListPage" view

@ClearSubscribers
Scenario: Verify that 1 Weekly and 1 Monthly test is displayed when its neither the last day of the month nor a Sunday and time is not between 08:00:00 and 23:59:59 
When for report purpose, today is not the last day of the month between 08:00:00 and 23:59:59 
And for report purpose, today is not sunday between 08:00:00 and 23:59:59 
And I refresh the cache
Then I navigate to the "QuizTrackerPage" view
And I can see the Quiz tracker with 1 weekly test and 1 monthly test

@ClearSubscribers
Scenario: Verify that 2 Weekly and 1 Monthly tests are displayed when today is a Sunday but not the last day of the month and time is between 08:00:00 and 23:59:59 
When for report purpose, today is not the last day of the month between 08:00:00 and 23:59:59 
And for report purpose, today is sunday between 08:00:00 and 23:59:59 
And I refresh the cache
Then I navigate to the "QuizTrackerPage" view
And I can see the Quiz tracker with 2 weekly test and 1 monthly test

@ClearSubscribers
Scenario: Verify that 1 Weekly and 2 Monthly tests are displayed when today is the last day of the month, but not a Sunday and time is between 08:00:00 and 23:59:59 
When for report purpose, today is the last day of the month between 08:00:00 and 23:59:59 
And for report purpose, today is not sunday between 08:00:00 and 23:59:59 
And I refresh the cache
Then I navigate to the "QuizTrackerPage" view
And I can see the Quiz tracker with 1 weekly test and 2 monthly test

@ClearSubscribers
Scenario: Verify that 2 Weekly and 2 Monthly tests are displayed when today is the last day of the month and a Sunday and time is between 08:00:00 and 23:59:59 
When for report purpose, today is the last day of the month and sunday between 08:00:00 and 23:59:59 
And I refresh the cache
Then I navigate to the "QuizTrackerPage" view
And I can see the Quiz tracker with 2 weekly test and 2 monthly test