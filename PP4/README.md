# Práctica4

## Nombre y carné:
###### Gabriel Arroyo Matamoros.
###### FI23029892.

***
## Comandos utilizados:
##### Verificación de Entity Framework
* `dotnet tool list --global`

##### Si no está, se ejecuta este
* `dotnet tool install --global dotnet-ef --version 9`

##### Creación del proyecto y su solución
* `dotnet new sln -n Practica4S`
* `dotnet new console -f net8.0 -o Practica4`
* `dotnet sln add Practica4/`

##### Agregar paquetes al proyecto
* `dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.0`
* `dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0`
* `dotnet add package CsvHelper`

##### Creación de la migración para la base de datos
* `dotnet build`
* `dotnet ef migrations add InitialCreate`
* `dotnet ef database update`
***

## Prompts:
- [Conversación.](https://chatgpt.com/share/6913c704-2bf0-8001-bf3b-ffe11dc850e1)
***

## Preguntas:

#### ¿Cómo cree que resultaría el uso de la estrategia de Code First para crear y actualizar una base de datos de tipo NoSQL (como por ejemplo MongoDB)? ¿Y con Database First? ¿Cree que habría complicaciones con las Foreign Keys?

Yo creo que sí resultaría con bases NoSQL porque utilizan colecciones y cada una tiene su tipo de dato, además de un ID único para cada fila que se crea. Sí habría complicaciones con las llaves foráneas, porque se emplean de forma diferente, como mencioné. Además, considero que sería más fácil Database First que Code First.

#### ¿Cuál carácter, además de la coma (,) y el Tab (\t), se podría usar para separar valores en un archivo de texto con el objetivo de ser interpretado como una tabla (matriz)? ¿Qué extensión le pondría y por qué? Por ejemplo: Pipe (|) con extensión .pipe.

Yo pienso que prácticamente cualquier símbolo o carácter que delimite valores puede servir. Por ejemplo: **:** o **$**. A esos dos ejemplos les pondría las extensiones **.dbdt** (que significa *double dot*) y **.dollr**, respectivamente.
