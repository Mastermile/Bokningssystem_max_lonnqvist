Detta är ett konsolbaserat bokningssystem som är utvecklat i C#. Det är designat för att hantera bokningar salar och grupprum. 
Systemet tillåter användare att skapa, visa och ta bort bokningar, samt att skapa nya och se befintliga lokaler.

Login: När man startar programmet måste man logga in, inloggningen består bara av ett användarnamn. Användarnamnet
används för att visa vem som bokat lokaler.

Lokaler: Användaren kan välja att skapa en ny lokal, programmet frågar efter vilken lokal typ man vill ha (grupprum eller sal).
Beroende på lokal typ kan man bara välje en viss kapacitet. Lokalen man skapar får automatiskt en ID designerat till sig så lokaler
kan ha samma namn men de har alltid ett unikt ID.

Bokningar: Användaren får välja mellan att boka en sal eller ett grupprum. Efter val visas alla lokaler i den kategori man valt.
Användaren bestämmer en start tid och slut tid innom en viss begränsning som är satt it programmets kod.
Användaren kan välja visa all bokningar och ta bort bokningar.


Begränsningar: Just nu sparas inte bokningarna inte permanent, så när programmet avslutas försvinner all data.
Man kan ta bort andra användares bokningar om det fanns fler användare.

