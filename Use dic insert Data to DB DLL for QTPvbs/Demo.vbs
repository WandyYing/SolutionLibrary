Server = "G2W4113.austin.hp.com,2048"
myDataBase = "AUTOMATION"
user = "aaadmin"
password = ""

GL_DB_DLL = "E:\SAP\Automation.dll"


set dic =CreateObject("Scripting.Dictionary")
set dic2 =CreateObject("Scripting.Dictionary")
dic.Add "ScenarioName", "Scenario 15"
'dic.Add "Dataversion", "USA7"
dic.Add "SAPBoxName", "NT2"
dic.Add "StartTime", Now 
dic.Add "Dataversion", "USA7"
dic.Add "TestInstance", 1

Set DBEngine = dotnetfactory.CreateInstance("Automation.DatabaseEngineFactory",GL_DB_DLL)

Set dbe = DBEngine.Create()
dbe.SetMapPath "E:\SAP\dbmap.txt"
dbe.Connect Server, myDataBase, user, password
dbe.InsertData  "dbo.Automation_Report", dic 
