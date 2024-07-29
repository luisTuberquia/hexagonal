# [El nombre de tu proyecto va aqu�]
![csharp-toolkit](https://img.shields.io/badge/.Net%208.0-Service-purple)
![csharp-toolkit](https://img.shields.io/badge/Architecture-Hexagonal-blue)
![csharp-toolkit](https://img.shields.io/badge/Dynamo-NoSQL-orange)
![csharp-toolkit](https://img.shields.io/badge/Docker-Container-yellow)
![csharp-toolkit](https://img.shields.io/badge/Cloud-AWS-orange)
![csharp-toolkit](https://img.shields.io/badge/Cloud-Microservices-blue)
![production-ready](https://img.shields.io/badge/In%20Develop-ready-green)

***
**Tabla de Contenido**
1. [Introducci�n](#id1)
2. [Levantando la Aplicaci�n](#id2)
    * 2.1. [Pre-Requisitos](#id2.1)
    * 2.2. [Ejecutando la Aplicaci�n](#id2.2)
    * 2.3. [Variables de Entorno](#id2.3)    
3. [Probando los servicios](#id3)
4. [Arquitectura de Aplicaciones](#id4)
    * 4.1. [Arquitectura Hexagonal](#id4.1)

***
Este proyecto se basa en la [**Arquitectura Hexagonal**](#id4) descrita en la secci�n 4 de este documento. El objetivo de este proyecto es usarlo como base para el desarrollo de las aplicaciones de FDLM que requieran una Arquitectura Hexagonal, una vez clonado, 
debes incluir tus propias clases en los m�dulos pre-configurados del proyecto. Tambi�n debes eliminar y adicionar los **M�dulos** que requiera tu proyecto.

La distribuci�n de m�dulos y paquetes del proyecto se basa en una Arquitectura Hexagonal e incluye ejemplos de servicios Rest.


<div id='id1' />

## 1. Introducci�n

[Se debe reemplazar esta introducci�n por una peque�a descripci�n de lo que hace el microservicio en general desde el punto de vista de negocio]

Este microservicio permite presentar en funcionamiento la Arquietectura de Referencia de Aplicaciones y su objetivo es que sirva como gu�a en la construcci�n de microservicios, mostrar ejemplos de implementaci�n de buenas pr�cticas de desarrollo y agilizar el desarrollo de microservicios.

Esta aplicaci�n de ejemplo expone 3 servicios Rest que permiten:
* Sumar n�meros enteros o complejos.
* Consultar las operaciones realizadas en un rango de fechas.
* Consultar una operaci�n por ID.

La aplicaci�n es capaz de persistir las operaciones en una base de datos embebida LiteDB o en DynamoDB y se us� Swagger para la documentaci�n de los servicios, para conocer como consumirlos se puede levantar la aplicaci�n y acceder a la URL: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html).

Adicionalmente la aplicaci�n tiene ejemplos sobre:
* Como usar Memory Cache para optimizar consultas a bases de datos.
* Como usar Polly para implementar Circuit Breaker en los llamados a servicios externos.
* Ejemplo de contenerizaci�n en Docker.
* Ejemplos de pruebas unitarias y de integraci�n.
* Inyecci�n de dependencias usando sustituci�n de liskov.
* Manejo de archivos de configuraci�n seg�n el ambiente de despliegue.
* Manejo de Conceptos de Abierto y Cerrado.

La aplicaci�n implementa la [**Arquitectura Hexagonal**](#id4) descrita en la secci�n 4 de este documento usando una sola soluci�n dividida en 5 proyectos independientes m�s 2 proyectos de soporte que no son parte de la arquitectura pero se necesitan para su ejecuci�n (FDLM.Runner y FDLM.Test). 

En la soluci�n, cada proyecto representa una capa de la arquitectura permitiendo separar y encapsular debidamente las responsabilidades de cada capa. Se recomienda revisar las dependencias entre los proyectos para evidenciar como las dependencias mapean la Arquitectura y la forma en la que se comunican las capas.

Al ser un proyecto de ejemplo que se usar� como base para otro proyecto, se debe tener en cuenta lo siguiente:
* Clonar este repositorio y copiar el c�digo en el nuevo repositorio.
* Cambiar el nombre a la soluci�n por la de tu microservicio.
* Buscar y Reemplzar todas las coincidencias de "ms-my-microservice" por el nombre del microservicio a construir.
* Personalizar la configuraci�n de Swagger del proyecto desde los archivos: 
  * /src/FDLM.Runner/Program.cs
  * /src/FDLM.Infrastructure.EntrypointsAdapters/Injections/SwaggerInjection.cs
* Personalizar los scripts de despliegue desde los archivos:
  * /Dockerfile
  * /src/FDLM.Runner/.gitlab-ci.yml
  * /src/FDLM.Runner/cd-ci-aws
* Modificar este Readme y adaptarlo a tu proyecto.

<div id='id2' />

## 2. Levantando la Aplicaci�n

La aplicaci�n permite ser ejecutada de forma local usando bases de datos locales, es posible usar dos tipos de bases de datos: una **LiteDB** que es usada para la ejecuci�n de **Pruebas de Integraci�n** y verificar funcionalidades de forma local, y tambi�n es posible configurar una base de datos **DynamoDB Local**, que permite verificar funcionalidades (no usar para pruebas de integraci�n).

Para configurar una u otra base de datos se debe cambiar el valor del campo "Database:Active" del archivo: **/src/FDLM.Runner/appsettings.local.json**, los posibles valores son: LiteDB o DynamoDB. Esto solo aplica para el ambiente **"local"**.

La aplicaci�n permite tener diferentes configuraciones seg�n el ambiente en el que se ejecute, los ambientes soportados son: 
* **local**: Usado para la construcci�n de la aplicaci�n e ideal para verificar funcionalidades de forma local, en este ambiente se usan bases de datos locales. 
* **dev**: Usado para la verificaci�n de la aplicaci�n en ambiente de desarrollo en AWS, debe usar servicios de nube.
* **qa**: Usado para la certificaci�n de la aplicaci�n por el equipo de aseguramiento de calidad.
* **uat**: Usado para la certificaci�n de la aplicaci�n por los usarios finale del cliente.
* **prod**: Usado para la puesta en marcha de la aplicaci�n y publicaci�n a usuarios finales.

La configuraci�n de cada ambiente se debe hacer desde los archivos **appsettings.[env].json** ubicados en el proyecto **/src/FDLM.Runner**. Para cambiar el ambiente de ejecuci�n se debe modificar el archivo **/src/FDLM.Runner/Properties/launchSettings.json** o estableciendo la variable de entorno **ASPNETCORE_ENVIRONMENT**.

<div id='id2.1' />

### 2.1. Pre-Requisitos
* Tener instalado [**Visual Studio Community 2022 o superior**](https://visualstudio.microsoft.com/vs/community/).
* Tener instalado [**.Net 8.0**](https://dotnet.microsoft.com/es-es/download/dotnet/8.0).
* Tener instalado [**Podman Desktop**](https://podman-desktop.io/downloads)
* Tener instalado Podman (Viene incluido en Podman Desktop), pero si lo quieres instalar manualmente lo puedes hacer desde aqu�: [**Podman**](https://podman.io/docs/installation). Si usas Windows y tienes problemas con la
  ejecuci�n de Podman, puedes ver esta [gu�a](https://blog.scottlogic.com/2022/02/15/replacing-docker-desktop-with-podman.html).
* Se requiere un servidor de **DynamoDB** ejecutandose en la m�quina local, para esto se recomienda usar una imagen de
  docker, la cual puedes obtener ejecutando el siguiente comando:
  ```shell script
  podman run --name dynamodb -p 8000:8000 -d amazon/dynamodb-local:latest -jar DynamoDBLocal.jar -inMemory -sharedDb
  ```
* Para la gesti�n de la base de datos DynamoDB se puede usar [**NoSQL Workbench by DynamoDB**](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.settingup.html)
* Para la gesti�n de la base de datos LiteDB se puede usar [**LiteDB Studio**](https://github.com/mbdavid/LiteDB.Studio/releases)

Si se seleccion� **DynamoDB Local** como base de datos, primero se debe crear la Tabla "CalculatroOperations", para esto pudes usar **NoSQL Workbench by DynamoDB**. 

Primero asegurate que la imagen de Docker de DynamoDB est� en ejecuci�n, despu�s con **NoSQL Workbench by DynamoDB** te conectas a la base de datos desde la opci�n **"Operation builder"** usando el string de conexi�n: http://localhost:8000.

Una vez conectado a la base de datos sigue los pasos de la imagen:

![Create Table](doc/table_creation.png)


<div id='id2.2' />

### 2.2. Ejecutando la Aplicaci�n
Una vez configurados los pre-requisitos mencionados anteriormente desde **Visual Studio** se establece como proyecto principal el proyecto **FDLM.Runner**, y se ejecuta la aplicaci�n.

Es recomendable que regularmente se verifique que la aplicaci�n se ejecuta adecuadamente dentro de un contenedor de Docker, debido a que las aplicaciones suelen comportarse diferente al ejecutarse de forma contenerizada. 

Para ejecutar la aplicaci�n de forma contenerizada se debe cerrar Visual Studio, abrir una consola, dirigirse a la raiz de la soluci�n y ejecutar los siguientes comandos:

Primero se construye la imagen:
```shell script
podman build -t my-mycroservice-img .
```

Se ejecuta la imagen:
```shell script
podman run -d -p 5000:8080 --name my-mycroservice-container my-mycroservice-img
```

**_IMPORTANTE:_** Con podman es posible que al ejecutar la imagen les genere errores del tipo **"The configured user limit (128) on the number of inotify instances has been reached"**, esto se debe a que el host de podman que levanta las imagenes no tiene configurado la propiedad **"fs.inotify.max_user_watches"**. Para configurarlo sigue los siguientes pasos:
* Abre una consola
* Asegurate de que podman est� ejecutandose, si no se est� ejecutando puedes hacerlo con este comando:
```shell script
podman machine start
```
* Ingresa a la m�quina por defecto de podman:
```shell script
podman machine ssh
```
* Con tu editor favorito de consola de linux (el m�o es vi) edita o crea el archivo **/etc/sysctl.conf**
* En ese archivo escribe la siguiente l�nea:
```shell script
fs.inotify.max_user_watches=524288
```
* Guarda el archivo y ejecuta el siguiente comando para que los cambios tomen efecto:
```shell script
sudo sysctl -p /etc/sysctl.conf
```
* Reinicia el servicio de contenedores y cierra la maquina virtual:
```shell script
sudo systemctl restart podman
exit
```
* Intenta volver a ejecutar la imagen.


<div id='id2.3' />

### 2.3. Variables de Entorno

Los secretos de la aplicaci�n se inyectan a trav�s de los archivos de configuraci�n **appsettings.[env].json** usando variables de entorno.

Cada ambiente (local, dev, qa, uat, prod) tiene sus propias variables de entorno. Para este proyecto en particular se usan
las siguientes variables de entorno:

**Para todos los ambientes:**
* **ASPNETCORE_ENVIRONMENT:** Esta variable indica el ambiente de ejecuci�n de la aplicaci�n, los posibles valores son: local, dev, qa, uat y prod.

[Aqu� se deben describir las variables de entorno necesarias para ejecutar la aplicaci�n]


<div id='id3' />

## 3. Probando los servicios

La aplicaci�n fue creanda usando Swagger, por lo tanto para acceder a la documentaci�n de los servicios se debe levantar la aplicaci�n en ambiente **local**, **dev**, o **qa** (No est� habilitado para UAT y Prod), y acceder a la URL [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

<div id='id4' />

## 4. Arquitectura de Aplicaciones

La Arquitectura de Aplicaciones es el referente para la implementaci�n de aplicaciones, servicios o microservicios y se centra en los componentes internos de la aplicaci�n y c�mo interact�an entre s�.
Considera las necesidades funcionales, los casos de uso espec�ficos de la aplicaci�n y el dise�o respeta y se acopla a los lineamientos establecidos en la Arquitectura de Soluciones.

La arquitectura (estructura) refleja el prop�sito del sistema, habla y modela los conceptos fundamentales de dominio, muestra claramente sus relaciones, refleja el prop�sito por el cual fue creada. En pocas palabras la arquitectura hace claro y accesible lo esencial.
Por lo anterior, para FDLM se decidi� usar una **Arquitectura Hexagonal** como base para la **Arquitectura de Referencia de Aplicaciones**.

<div id='id4.1' />

### 4.1. Arquitectura Hexagonal

El estilo de **Arquitectura Hexagonal** separa la l�gica de negocio de la infraestructura sobre la que se soporta, esto ayuda a dise�ar aplicaciones con muy bajo acoplamiento y una alta cohesi�n. De esa manera, las aplicaciones ser�n m�s portables, f�ciles de probar y flexibles para cambiar a medida que crezcan los proyectos o se requieran cambios en la infraestructura.

Principios de Dise�o y Modelado
Con el fin de identificar la estructura y l�mites adecuados del sistema se recomienda utilizar las pr�cticas de dise�o estrat�gico presentadas en el enfoque de dise�o dirigido por el dominio DDD (Domain-Driven Design, by Eric Evans):
* Identificar el Core Domain.
* Identificar los distintos Subdominios que soportan el Core Domain.
* Cada uno de los Subdominios identificados al igual que el Core Domain se materializan como un Contexto delimitado (Bounded Context) el cual debe tener completamente definido un lenguaje com�n (Ubiquitous Language) (lo cual ser� la base para las entidades y elementos del Dominio).
* Con cada contexto delimitado se tiene la base fundamental que define cada uno de los microservicios que se implementar�n para el sistema. Se debe empezar con una correspondencia 1-1 en caso de tener muy clara la separaci�n de dichos contextos; si por el contrario la delimitaci�n no es completamente clara al inicio y no existen equipos y expertos del dominio independientes para dichos contextos se recomienda empezar con una sola base de c�digo que podr� ser segregada en distintos microservicios al momento que se tenga mayor claridad.
* Se recomienda el Event Storming como actividad de modelado.

La representaci�n m�s com�n de estas arquitecturas es un diagrama que consta de capas circulares conc�ntricas donde las capas internas representan objetos y procesos de l�gica de negocio, mientras que las capas externas representan detalles t�cnicos de implementaci�n, tecnolog�as y frameworks, como por ejemplo. bases de datos, protocolos de comunicaci�n, dispositivos de almacenamiento, hardware, etc.
La siguiente Figura muestra la adaptaci�n de las Arquitecturas Hexagonales para FDLM y es la base principal de todos los microservicios que manejan casos de negocio.

![Hexagonal Architecture](doc/hexagonal_architecture.png)

Los componentes de la arquitectura son los siguientes:
* **Domain:** Encapsula componentes de negocio y funcionalidades transversales que est�n disponibles para toda la aplicaci�n.      
    * **Models**: Contiene los objetos o modelos de negocio de la aplicaci�n.
    * **Services**: Contiene los servicios que implementan las reglas de negocio, manipulan los modelos y realizan operaciones sobre ellos. Los Servicios exponen la funcionalidad del n�cleo del negocio hacia las capas superiores, los servicios contienen l�gicas o casu�sticas reutilizables y de apoyo que permiten modificar los datos. Los Servicios solo procesan objetos del Modelo de dominio y no tienen ninguna relaci�n o dependencia con interfaces u objetos de infraestructura como: base de datos, servicios rest, etc.
* **Application:** Encapsula las interfaces (Puertos) que permiten comunicar el dominio con la infraestructura, desacoplando as� la l�gica de negocio sobre la tecnolog�a subyacente que se utiliza en la aplicaci�n. Adicionalmente contiene los Use Cases que permiten orquestar los Servicios de dominio junto con las Interfaces de Infraestructura.
    * **Ports:** Presentan una capa de abstracci�n que permite acceder a la infraestructura (interfaces), la l�gica de negocio es implementada en la infraestructura, a estas implementaciones se les denomina **adaptadores**, los cuales realizan operaciones sobre la infraestructura, como por ejemplo, persistir un dato. Los **Ports** no est�n amarrados a una tecnolog�a en particular, por lo tanto la l�gica de negocio cuando quiere persistir un dato no sabe si ese dato se va a almacenar en una base de datos, en un archivo o se va a enviar por un servicio Rest para ser almacenado en un sistema externo.
    * **Use Cases:** Contiene la l�gica que orquesta los diferentes **Servicios** de dominio y los integra con los **Ports** para generar la l�gica final que expone la aplicaci�n hacia el mundo exterior. Los **UseCase** tambi�n tiene l�gica de negocio pero orientada m�s la adapataci�n de los servicios a escenarios funcionales espec�fcios de negocio, los cuales son m�s subsetibles a cambiar con el tiempo que los Servicios de Dominio. Los UseCase, al igual que los Servicios de Dominio, solo procesan objetos del Modelo de dominio.
* **Infrastructure:** Encapsula componentes asociados directamente a una tecnolog�a, framework o hardware en particular, por ejemplo: Bases de datos, servicios externos, driver para acceder a hardware del dispositivo o servidor, unidades de almacenamiento de archivos, etc.
    * **EntrypointsAdapters:** Encapsula tecnolog�as o frameworks empleados para recibir peticiones o mensajes desde el mundo exterior. Los Entrypoints reciben la informaci�n en DTOs, los cuales se transforman en objetos del Modelo de dominio para ser procesados por los **UseCases**. Se recuerda que los **UseCase** solo manipulan objetos de dominio.
        * **Rest Controller Services:** Permite recibir mensajes por servicios Rest usando DTOs.
        * **Broker Messages Consummer:** Permite recibir mensajes por Brokers de Mensajer�a usando DTOs.
    * **OutpointsAdapters:** Encapsula tecnolog�as o frameworks para persistir datos, obtener datos desde sistemas externos o enviar datos a otros sistemas.
        * **Data Base Adapters:** Permite almacenar y obtener datos en una Base de Datos.
        * **File Storage Adapters:** Permite almacenar y obtener datos en archivos de texto en disco duro o servicio externo.
        * **Rest Adapters**: Son endpoints los cuales se consumen por servicios Rest para el env�o o solicitud de informaci�n, son usados para la integraci�n con sistemas externos.
        * **Broker Message Producer Adapters**: Produce mensajes hacia un Borker de Mensajer�a para el env�o o solicitud de informaci�n, son usados para la integraci�n con sistemas externos.
* **Utilities:** Contiene utilidades que pueden ser utilizadas por toda la aplicaci�n, no dependen de la infraestructura, tampoco referencian l�gica de negocio.

**Principios Estructurales que se deben Seguir**
El principio estructural b�sico que determinar� la estructura general de los microservicios son:
* La l�gica de bajo nivel no debe depender de logicas de alto nivel o lo que se llamar�n detalles de implementaci�n tecnol�gicamente concretos.
* No deben existir dependencias c�clicas, debe existir coherencia con el principio ADP (acyclic dependencies principle).
* La estructura interna de los microservicios deber� ser una estructura modular que permita separar y agrupar el c�digo fuente en m�dulos que expresen su prop�sito y agrupen clases de acuerdo con el principio CCP (Common Closure Principle).
* Este agrupamiento modular debe seguir los principios de una arquitectura centrada en el dominio (Hexagonal Architecture (a.k.a. Ports and Adapters) by Alistair Cockburn), como se ilustra en la Figura de Arquitectura de Referencia de Aplicaciones, teniendo en el centro de todo el grafo de dependencias los m�dulos que representan las entidades y elementos del dominio en donde debe residir la l�gica que representa las reglas de negocio cr�ticas, pol�ticas y flujos de negocio de alto nivel (Use Cases).
* Los m�dulos que residen en la capa de dominio no deben tener dependencias hacia los componentes que se encuentran en las capas m�s externas ya que dichos componentes representan puertos y adaptadores a tecnolog�as y subsistemas externos, los cuales por su naturaleza son vol�tiles y deben poder ser f�cilmente intercambiables.
* Los adaptadores externos pertenecen a una de las siguientes categor�as, o son puntos de entrada al sistema, es decir disparan se�ales, eventos o comandos a los cuales el dominio reacciona o son adaptadores a necesidades de interacci�n con subsistemas, bases de datos o tecnolog�as externas. Los m�dulos en esta capa pueden depender de diversas tecnolog�as externas. Seg�n sea el caso las reglas de dependencias de estos m�dulos var�a:
    * **EntryPoints:** Tambi�n conocidos c�mo Outpoints primarios, dependen del m�dulo domain:useCases y su comportamiento est� limitado a adaptar y transformar los datos de entrada a un formato que sea compatible con el lenguaje de dominio para posteriormente disparar la ejecuci�n del proceso de dominio, no deben tener ninguna dependencia con ning�n otro EntryPoint ni mucho menos depender directamente de ning�n otro Outpoints ya que esto abre la puerta a que se implemente l�gica de dominio en las capas externas. 
        * Deben delegar la ejecuci�n de l�gica completamente en los UseCases.
        * No deben depender de Outpoints ni interfaces definidas en el dominio.
        * No deben implementar interfaces que ser�n llamadas por el dominio (ya que este comportamiento es propio de los Outpoints)
    * **Outpoints:** Dependen de los m�dulos domain:model e infrasructure:adapters que es en �ste �ltimo donde se encuentran las interfaces que el adaptador implementar�, su responsabilidad es adaptar y traducir al lenguaje de dominio, interacciones con subsistemas o tecnolog�as ajenas al dominio, estos m�dulos son pasivos, es decir, no llaman al dominio, sino que materializan de una forma tecnol�gicamente concreta las necesidades expresadas por este en su propio lenguaje, por ejemplo: persisten un objeto del modelo de dominio transform�ndolo en una o varias entidades que representan una o varias tablas en una base de datos relacional.

**Modelado Interno del Dominio**
* En el m�dulo domain:model deben residir los modelos y elementos que representan el lenguaje ubicuo del contexto delimitado, entre dichos elementos est�n: Modelos (representan los sujetos del dominio), constantes, excepciones, enumeraciones y diversas estructuras de datos que representan aspectos del dominio.
* En los elementos del dominio no deben existir referencias sem�nticas a tecnolog�as externas, as� �stas sean meramente definiciones de interfaces, ejemplo: AWSSMSSender, en lugar de ello debe ser definido: NotificationsSender.
* Emplear el concepto de Agregados de DDD: En el contexto de Domain-Driven Design (DDD), un agregado es un patr�n fundamental que ayuda a organizar y modelar las relaciones entre objetos en un sistema. Estos son los conceptos clave:
    * Un agregado es un conjunto de entidades y objetos de valor que se agrupan en una unidad coherente.
    * Representa una transacci�n at�mica y una unidad de consistencia en el dominio.
    * Los agregados son la unidad b�sica de transacci�n y consistencia en DDD.
    * Ra�z del Agregado: Cada agregado tiene una entidad ra�z que act�a como punto de entrada para acceder a otros objetos dentro del agregado.
    * La ra�z del agregado garantiza la integridad y la consistencia de todo el conjunto.
    * Las operaciones de lectura y escritura deben realizarse dentro de los l�mites del agregado.
* Las transacciones no deben cruzar los l�mites del agregado.
* Ejemplo:
  * En un sistema de comercio electr�nico, un pedido y sus l�neas de pedido pueden formar un agregado.
  * El pedido ser�a la ra�z del agregado, y las l�neas de pedido estar�an contenidas dentro de �l.
* Los agregados son una herramienta importante para modelar y encapsular la l�gica de negocio de manera efectiva, garantizando la coherencia y la integridad en el dominio.