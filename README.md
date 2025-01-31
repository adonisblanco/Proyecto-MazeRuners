# Proyecto-MazeRuners
Pasos para empezar con el proyecto(Informe):
1. Descargar el Visual Studio 2022
2. Descargar preferiblemente el community en google
3. Abrirlo y crear un nueo proyecto con c sharp
4. En el juego en el formulario Reglas se explican las demás cosas del gameplay
Así que este es el informe:
Descripción General
El proyecto consiste en un juego de laberinto multijugador desarrollado en C# utilizando Windows Forms, donde los jugadores compiten por llegar a la meta mientras evitan trampas y utilizan habilidades especiales.

Características Principales
1. Sistema de Laberinto
  Generación automática de un laberinto 15x15
  Múltiples caminos posibles hacia la meta
  Sistema de trampas distribuidas aleatoriamente
  Marcadores de inicio (S) y meta (E)
2. Sistema de Jugadores
  Capacidad para 1-5 jugadores
  Símbolos únicos para cada jugador: ♠,♣,♥,♦,$
  Posicionamiento aleatorio inicial
  Sistema de turnos rotativos
3. Habilidades Especiales
   Cada jugador tiene una habilidad única:

 Jugador 1: Aumenta 3 movimientos
 Jugador 2: Inmunidad a trampas
 Jugador 3: Envía al siguiente jugador al inicio
 Jugador 4: Revela trampas cercanas
 Jugador 5: Duplica movimientos del siguiente turno
4. Sistema de Trampas
  Se implementaron diferentes tipos de trampas:

  Retorno al inicio
  Salto de turno
  Teletransporte aleatorio
5. Mecánicas de Juego
   Movimiento
   Sistema basado en turnos
   4-5 movimientos por turno (aleatorio)
   Validación de movimientos permitidos
   Detección de colisiones con paredes
   Condiciones de Victoria
  Llegar a la casilla marcada como 'E'
  Mensaje de victoria personalizado
  Opción de reinicio de partida
6. Conclusiones
El juego combina elementos estratégicos con factores aleatorios, creando una experiencia dinámica y rejugable. La implementación técnica permite una fácil expansión y modificación de características.

Tecnologías Utilizadas
C# como lenguaje principal
Windows Forms para la interfaz gráfica
.NET Framework como plataforma base
   
