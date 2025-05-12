# TizenSigner

Tool voor het installeren van Club Info Board op je Samsung Smart TV

## Vereisten
- Samsung Tizen TV (met ontwikkelmodus ingeschakeld)
- Het IP-adres van de TV
- Netwerkverbinding tussen je computer en de TV

## Stappen voor gebruik

### 1. Developer Mode inschakelen op Samsung TV
Volg deze stappen om ontwikkelmodus in te schakelen:
1. Druk op de **Home**/**Smart Hub**-knop van je afstandsbediening
2. Ga naar **Apps**
3. In het Apps-menu, voer het nummer **12345** in met de afstandsbediening of het on-screen toetsenbord
   - Dit opent een nieuw menu
4. Schakelen **Ontwikkelaarsmodus** in
5. Voer bij de optie "IP-adres" het IP-adres in van je computer (waar TizenSigner draait)
   - Als je die niet weet, open je command prompt en voer in `ipconfig` (Windows) of open terminal `ifconfig` (Mac/Linux) in om je lokale IP-adres te vinden

### 2. IP-adres van de TV vinden
1. Ga naar **Instellingen** > **Algemeen** > **Netwerk** > **Netwerkstatus**
2. Het IP-adres staat vermeld onder "IP-adres" of "Netwerkinformatie"
   - Bijvoorbeeld: `192.168.1.100`

### 3. TizenSigner gebruiken
1. Verbind je computer met hetzelfde netwerk als de TV
2. Voer het IP-adres in de tool in
3. Als de app geinstalleerd is wordt deze automatisch geopend door de TV

## Ondersteuning
Problemen? Open een issue op onze GitHub repository.