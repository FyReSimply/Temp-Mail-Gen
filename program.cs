using SmorcIRL.TempMail;
using SmorcIRL.TempMail.Models;
using System.Reflection.Metadata.Ecma335;

// Initialize the client once
MailClient client = new MailClient(new Uri("https://api.mail.gw/"));

bool currentServer = false;

string banner = @"

  _________.__               .__                       _____         .__.__      ________               
 /   _____/|__| _____ ______ |  | ___.__.             /     \ _____  |__|  |    /  _____/  ____   ____  
 \_____  \ |  |/     \\____ \|  |<   |  |   ______   /  \ /  \\__  \ |  |  |   /   \  ____/ __ \ /    \ 
 /        \|  |  Y Y  \  |_> >  |_\___  |  /_____/  /    Y    \/ __ \|  |  |__ \    \_\  \  ___/|   |  \
/_______  /|__|__|_|  /   __/|____/ ____|           \____|__  (____  /__|____/  \______  /\___  >___|  /
        \/          \/|__|        \/                        \/     \/                  \/     \/     \/ 
                                                            github.com/FyReSimply";
Console.WriteLine(banner);

Console.WriteLine("How many emails do you want to generate?: ");
string input = Console.ReadLine();

Console.WriteLine("Delete Last Saves ? [y/n] Default : no ");

string clearFile = Console.ReadLine();
bool doClear = false;
if(clearFile == "y")
{
    doClear = true;
} else doClear = false;

if (doClear) File.Delete("EmailInfos.txt");
else { }

// Convert the string input to an integer
if (int.TryParse(input, out int numEmailInt))
{
    // We call the async method and wait for it to finish
    await GenEmails(numEmailInt, client);
}

// Note: Function is "async Task" so we can use "await" inside it
async Task GenEmails(int count, MailClient mailClient)
{
    for (int i = 0; i < count; i++)
    {
        if (currentServer)
            client = new MailClient(new Uri("https://api.mail.gw"));
        if (!currentServer)
            client = new MailClient(new Uri("https://api.mail.tm"));

        Thread.Sleep(1000);
        // 1. Get domain
        DomainInfo[] domains = await mailClient.GetAvailableDomains();
        string domain = domains[0].Domain;

        // 2. Setup credentials
        string myEmail = $"user{Guid.NewGuid().ToString()[..5]}@{domain}";
        string myPassword = "SimplySecurePassword123!";

        // 3. Register and Login
        try
        {
            // Your generation code here...
            await mailClient.Register(myEmail, myPassword);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Changing api...");
            currentServer = !currentServer;
            domain = domains[i].Domain;
            continue; // This jumps to the next iteration of the 'for' loop
        }

        // 4. Fetch info
        AccountInfo info = await mailClient.GetAccountInfo();

        Console.WriteLine($"--- Email #{i + 1} ---");
        Console.WriteLine($"Address: {info.Address}");
        Console.WriteLine($"Password: {myPassword}");
        Console.WriteLine("---------------------");

        File.AppendAllText("EmailInfos.txt", $"--- Email #{i + 1} ---\nAddress: {info.Address}\nPassword : {myPassword}\n---------------------\n");
    }
}
