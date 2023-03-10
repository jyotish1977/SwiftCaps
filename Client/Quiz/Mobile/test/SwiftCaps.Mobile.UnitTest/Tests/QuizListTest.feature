Feature: Quiz List Page

Background: 
    Given I am an unauthenticated user
    When I login with Azure AD
	Then I am on the "QuizListPage" view 
	
@ClearSubscribers
Scenario: Get Incomplete User Quizzes
	When today is not the last day of the month between 08:00:00 and 23:59:59
	And today is not sunday between 08:00:00 and 23:59:59
    And I refresh the cache
    And I reload the quizzes
	Then I can see a "Incomplete" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Incomplete" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"  

@ClearSubscribers
Scenario: Get Incomplete User Quizzes at end of month
	When today is the last day of the month between 08:00:00 and 23:59:59 
	And today is not sunday between 08:00:00 and 23:59:59
    And I refresh the cache
    And I reload the quizzes
	Then I can see a "Incomplete" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Incomplete" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"
	And I can see a "Incomplete" "Next" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 

@ClearSubscribers
Scenario: Get Incomplete User Quizzest at end of week
	When today is not the last day of the month between 08:00:00 and 23:59:59 
	And today is sunday between 08:00:00 and 23:59:59
    And I refresh the cache
    And I reload the quizzes
	Then I can see a "Incomplete" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Incomplete" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"  
	And I can see a "Incomplete" "Next" "Weekly" quiz card with title "F-16 CAPs" 
		
@ClearSubscribers
Scenario: Get Incomplete User Quizzes at end of week and end of month
	When today is the last day of the month and sunday between 08:00:00 and 23:59:59
    And I refresh the cache
    And I reload the quizzes
	Then I can see a "Incomplete" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Incomplete" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"
	And I can see a "Incomplete" "Next" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"
	And I can see a "Incomplete" "Next" "Weekly" quiz card with title "F-16 CAPs"

@ClearSubscribers
Scenario: Get Completed current Weekly and Monthly Quizzes
	When today is not the last day of the month between 08:00:00 and 23:59:59 
	And today is not sunday between 08:00:00 and 23:59:59
    And I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Weekly" Quiz 
	And I completed the "Current" "Monthly" Quiz 
	Then I can see a "Completed" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Completed" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 

@ClearSubscribers
Scenario: Get Completed current Weekly and Incomplete Monthly Quizzes
    When today is not the last day of the month between 08:00:00 and 23:59:59 
	And today is not sunday between 08:00:00 and 23:59:59
    And I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Weekly" Quiz 
    And The "Current" "Monthly" is incomplete
	Then I can see a "Completed" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Incomplete" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 

@ClearSubscribers
Scenario: Get Incomplete current Weekly and Completed Monthly Quizzes
	When today is not the last day of the month between 08:00:00 and 23:59:59 
	And today is not sunday between 08:00:00 and 23:59:59
    And I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Monthly" Quiz 
    And The "Current" "Weekly" is incomplete
	Then I can see a "Incomplete" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Completed" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 

@ClearSubscribers
Scenario: Get Completed current Weekly and Monthly Quizzes and Incomplete Weekly and Monthly Quizzes at end of week and end of month
	When today is the last day of the month and sunday between 08:00:00 and 23:59:59 
    And I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Weekly" Quiz 
	And I completed the "Current" "Monthly" Quiz 
    And The "Next" "Monthly" is incomplete
    And The "Next" "Weekly" is incomplete
	Then I can see a "Completed" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Completed" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 
	And I can see a "Incomplete" "Next" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"
	And I can see a "Incomplete" "Next" "Weekly" quiz card with title "F-16 CAPs"

@ClearSubscribers
Scenario: Get Completed current Weekly and Monthly Quizzes and Completed Weekly and Monthly Quizzes at end of week and end of month
	When today is the last day of the month and sunday between 08:00:00 and 23:59:59 
    And I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Weekly" Quiz 
	And I completed the "Current" "Monthly" Quiz 
	And I completed the "Next" "Weekly" Quiz 
	And I completed the "Next" "Monthly" Quiz 
	Then I can see a "Completed" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Completed" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 
	And I can see a "Completed" "Next" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"
	And I can see a "Completed" "Next" "Weekly" quiz card with title "F-16 CAPs"


@ClearSubscribers
Scenario: Tap on a weekly and monthly completed quiz does nothing
    When I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Weekly" Quiz 
	And I completed the "Current" "Monthly" Quiz 
	When I tap on the expired "Current" "Weekly" Quiz card
	Then I am on the "QuizListPage" view
	When I tap on the expired "Current" "Monthly" Quiz card
	Then I am on the "QuizListPage" view


@ClearSubscribers
Scenario: Get Incomplete current Weekly Quizzes After completing last week quiz
	When today is the first day of last week
    And I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Weekly" Quiz 
	Then I can see a "Completed" "Current" "Weekly" quiz card with title "F-16 CAPs" 
    When today is the first day of this week
    And I refresh the cache
    And I reload the quizzes
    And The "Current" "Weekly" is incomplete
    Then I can see a "Incomplete" "Current" "Weekly" quiz card with title "F-16 CAPs" 

@ClearSubscribers
Scenario: Get Incomplete current Monthly Quizzes After completing last month quiz
	When today is the first day of last month
    And I refresh the cache
    And I reload the quizzes
	And I completed the "Current" "Monthly" Quiz 
	Then I can see a "Completed" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 
    When today is the first day of this month
    And I refresh the cache
    And I reload the quizzes
    Then I can see a "Incomplete" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)" 


@ClearSubscribers
Scenario: Get one Incomplete weekly quiz, one Complete weekly quiz and one Incomplete Monthly Quiz
	When today is not the last day of the month between 08:00:00 and 23:59:59 
	And today is sunday between 08:00:00 and 23:59:59 
    And I refresh the cache
    And I reload the quizzes
    And I completed the "Current" "Weekly" Quiz
    And The "Next" "Weekly" is incomplete
    And The "Current" "Monthly" is incomplete
	Then I can see a "Completed" "Current" "Weekly" quiz card with title "F-16 CAPs" 
	Then I can see a "Incomplete" "Next" "Weekly" quiz card with title "F-16 CAPs" 
	And I can see a "Incomplete" "Current" "Monthly" quiz card with title "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)"
