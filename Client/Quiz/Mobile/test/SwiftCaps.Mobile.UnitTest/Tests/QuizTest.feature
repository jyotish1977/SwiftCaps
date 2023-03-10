Feature: Quiz Page


Background:
Given I am an unauthenticated user
When I login with Azure AD
Then I am on the "QuizListPage" view
When today is not sunday between 08:00:00 and 23:59:59
And I refresh the cache
And I reload the quizzes
Then I can see a "Incomplete" "Current" "Weekly" quiz card with title "F-16 CAPs"
When I tap on the "Current" "Weekly" Quiz card
Then I am redirected to the "QuizPage" view
And the user quiz has been created


@ClearSubscribers
Scenario Outline: Start a quiz
Given I am on the "<Recurrence>" quiz
And I am on the quiz section 1 out of <QuizSectionCount>
And I am on the question 1 out of <QuestionCount>
Then I can see a label "CurrentSection.Description" with text "<Description>"
And I can see a label "CurrentQuestion.Header" with text "<Header>"
And I can see a label "CurrentQuestion.Footer" with text "<Footer>"
And I can see 1 answers placeholders
And I can see a disabled button "SubmitSection"

When I enter "<BadAnswer>" in the answer 0
Then I can see the answer 0 is invalid
And I can see a disabled button "SubmitSection"
When I enter "<GoodAnswer>" in the answer 0
Then I can see the answer 0 is valid
And I can see all the answers on the question 1 are valid
And I can see a disabled button "SubmitSection"

When I navigate to the next question
Then I am on the quiz section 1 out of <QuizSectionCount>
And I am on the question 2 out of <QuestionCount>
When I correctly answer all the answers on the question 2
Then I can see all the answers on the question 2 are valid
And I can see a disabled button "SubmitSection"

When I correctly answer all the answers on the quiz section 1
Then I can see all the answers on the quiz section 1 are valid
And I can see an enabled button "SubmitSection"

When I tap a button "SubmitSection"
Then I am on the quiz section 2 out of <QuizSectionCount>

Examples:
| Recurrence | BadAnswer | GoodAnswer   | QuizSectionCount | QuestionCount | Description                       | Header | Footer | Body                 |
| Weekly     | test      | THROTTLE OFF | 10               | 3             | FIRE/OVERHEAT/FUEL LEAK/ (GROUND) |        |        | 1. 1: ______________ |


@ClearSubscribers
Scenario Outline: Submit a quiz
Given I am on the "<Recurrence>" quiz
Then the quiz is not complete
And I am on the quiz section 1 out of <QuizSectionCount>
And I am on the question 1 out of <QuestionCount>
When I correctly answer all the questions of the "<Recurrence>" quiz
Then the quiz is complete
And I can see a popup with header "<PopupTitle>" and message "<PopupMessage>"
And I take note of the completed quiz id
And I am redirected to the "QuizListPage" view
And I can see a "Completed" "Current" "<Recurrence>" quiz card with title "F-16 CAPs"
And I can see the completed quiz with the id I took note of

Examples:
| Recurrence | PopupTitle           | PopupMessage                                  | QuizSectionCount | QuestionCount |
| Weekly     | Congratulations Alan | You successfully completed the F-16 CAPs Quiz | 10               | 3             |

@ClearSubscribers
Scenario Outline: Tapping on Quiz Info
Given I am on the "<Recurrence>" quiz
And I am on the quiz section 1 out of <QuizSectionCount>
And I am on the question 1 out of <QuestionCount>
When I tap a button "OpenInfo"
Then I see that pop up is displayed
When I click on the close button
Then I am on the quiz section 1 out of <QuizSectionCount>

Examples:
| Recurrence | QuizSectionCount | QuestionCount |
| Weekly	 | 10				| 3				|