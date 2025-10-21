# Practica3

## Nombre y carné:
###### Gabriel Arroyo Matamoros.
###### FI23029892.

***
## Comandos utilizados:
* dotnet new sln --name Practica3S
* dotnet new webapi -f net8.0 -o Practica3
* dotnet sln add Practica3
* cd Practica3
* dotnet run
***

## Prompts:
- [Conversación.](https://chatgpt.com/share/68f811e9-efd4-8001-89f0-8de047a27ef8)
***

## Preguntas:

#### ¿Es posible enviar valores en el Body (por ejemplo, en el Form) del Request de tipo GET?

Sí, es posible; una vez, en Programación Avanzada, un profesor hizo un ejemplo de ello, pero no se debe realizar nunca. Probando en Postman, noté que solo se puede enviar texto o archivos, por lo que, en el caso de las APIs (como esta Minimal API), no es posible enviar datos de esa forma.

#### ¿Qué ventajas y desventajas se observan con el Minimal API si se compara con la opción de utilizar Controllers?

La ventaja es su simplicidad y, además, es más fácil de entender desde mi perspectiva. Como desventaja, en proyectos grandes no sería adecuado; como se mencionó en clase, está pensado principalmente para microservicios o proyectos pequeños.
