using DotNet.FindConflictingReferences.ClassProgram;

string pathLog = @"C:\ReferenceLog";

Console.WriteLine("Digite o caminho da pasta bin do projeto: ");
string pathBin = Console.ReadLine();

var find = new FindConflictingReferences();
var saveLog = find.FindConflicting(pathBin, pathLog);

if (saveLog)
    Console.WriteLine($@"Log gravado com sucesso em: {pathLog}");
else
    Console.WriteLine("Erro ao gravar o log!");

Console.ReadKey();