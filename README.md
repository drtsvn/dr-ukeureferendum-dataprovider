dr-ukeureferendum-dataprovider
==============================

Dette er en simpel consol applikation der kun forventes at skulle bruges i et par døgn i forbindelse med EU-afstemningen i Storbritannien d. 23/6-16. Derfor er det en quick-and-dirty løsning.

I de relevante døgn kommer applikationen til at køre på proc02 via et SQL Server Agent job.  Hver gang applikationen kører henter den senesete satmmentællingsdata fra en ftp-server, parser dem og sender dem i Firebase. 
