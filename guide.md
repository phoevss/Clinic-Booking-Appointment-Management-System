🛡️ Step 1: The Admin (Setting up the Schedule)
The Admin must first "open" the clinic/business for bookings by adding available time slots.

Run FrontDeskApp: Use dotnet run --project FrontDeskApp\FrontDeskApp.csproj.
Login: Use admin / admin123.
Add Availability:
Click Manage Schedules on the left.
Select a Service (e.g., General Consultation).
Pick a Date (e.g., Today).
Enter a Time (e.g., 09:00).
Click Add Time Slot. (Repeat this to add several slots like 10:00, 11:00, etc.).
👤 Step 2: The User (Making a Reservation)
The User can now see and book the slots created by the Admin.

Run UserApp: Use dotnet run --project UserApp\UserApp.csproj.
Register/Login: Create a new account or log in with an existing one.
Book an Appointment:
Click New Booking on the left.
Select the Service and Pick the Date you just set up as Admin.
Click CHECK AVAILABILITY.
The slots (like 09:00) will appear in the list. Click on a slot to select it.
Click the green CONFIRM BOOKING button.
Wait for Approval: Your booking is now sent to the Admin for review. You can see it in My History as "Pending."
🛡️ Step 3: The Admin (Approving the Booking)
The Admin reviews the request and confirms it.

Go back to FrontDeskApp.
Approve the Request:
Click Pending Approvals on the left.
You will see the user's booking in the list.
Select the row and click the green APPROVE button at the bottom.
📊 Step 4: Reporting (Admin Only)
Check the overall statistics of the system.

Click Reports Dashboard in the FrontDeskApp.
Select "Daily" or "Monthly" and click Generate Report.
You can click Export to PDF to simulate generating a professional report for your final project.
Summary of Workflow: Admin (Add Slots) ➡️ User (Book Slot) ➡️ Admin (Approve) ➡️ Done! ✅