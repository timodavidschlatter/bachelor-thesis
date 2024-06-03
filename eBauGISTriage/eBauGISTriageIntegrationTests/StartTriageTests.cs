using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using eBauGISTriageApi;
using eBauGISTriageApi.Controllers;
using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Models;
using eBauGISTriageApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Xunit.Abstractions;

namespace eBauGISTriageIntegrationTests
{
    /// <summary>
    /// This class contains all integration tests for the startTriage of eBauGISTriageApi.
    /// </summary>
    public class StartTriageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly JsonSerializerOptions _JSON_SERIALIZER_OPTIONS = new() { PropertyNameCaseInsensitive = true, };

        private const string c_apiUrl = "api/Triage/startTriage";
        private const string c_loadQueriesUrl = "api/Triage/loadQueries";
        private const string c_jsonRequestBody = "{ \"shapes\" : [ \"POLYGON((2627973.6339951 " +
            "1254304.548002,2627980.2419907 1254307.8530007,2627988.3290024 1254291.5920022," +
            "2627980.937999 1254288.1130044,2627975.0249985 1254299.6790017,2627975.9809922 1254299.7660012" +
            ",2627973.6339951 1254304.548002))\" ], \"points\": [ \"POINT(2627981.111998 1254296.9829995)\" ],\"strings\":{\"baugesuch_prj_ort1\":\"Lausen\"},\"integers\":{\"baugesuch_gebauedetyp_id\":210}}";

        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private readonly MediaTypeWithQualityHeaderValue _mediaTypeHeaderValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartTriageTests"/> class.
        /// </summary>
        /// <param name="factory">The web application factory.</param>
        /// <param name="output">The test output helper.</param>
        public StartTriageTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            this._client = factory.CreateClient();
            this._output = output;
            this._mediaTypeHeaderValue = new(MediaTypeNames.Application.Json);
            this._client.DefaultRequestHeaders.Accept.Add(this._mediaTypeHeaderValue);
        }

        /// <summary>
        /// Agile story 1 test nr. 1.
        /// </summary>
        /// <param name="mediaType">The mime type in the _request Content-Type header.</param>
        [Theory]
        [InlineData(MediaTypeNames.Text.Plain)]
        [InlineData(MediaTypeNames.Application.Octet)]
        public async void Post_NotSupportedContentType_ReturnUnsupportedMediaType(string mediaType)
        {
            // Arrange
            var requestContent = new StringContent(string.Empty, Encoding.UTF8, mediaType);

            // Act
            var response = await this._client.PostAsync(c_apiUrl, requestContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }

        /// <summary>
        /// Agile story 1 test nr. 2.
        /// </summary>
        /// <param name="jsonRequestBody">The JSON string in the _request body.</param>
        [Theory]
        [InlineData("")]
        [InlineData("{foo:bar}")]
        [InlineData("{\"shapes\":\"POLYGON((2627973.6339951 1254304.548002))\"," +
            "\"points\":\"POINT(2627981.111998 1254296.9829995)\"}")]
        public async void Post_DifferentFalseBodyFormats_ReturnBadRequest(string jsonRequestBody)
        {
            // Arrange
            var requestContent = new StringContent(jsonRequestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

            // Act
            var response = await this._client.PostAsync(c_apiUrl, requestContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Agile story 1 test nr. 3.
        /// </summary>
        [Fact]
        public async void Post_NotSupportedAccept_ReturnNotAcceptable()
        {
            // Arrange
            var requestContent = new StringContent(c_jsonRequestBody, Encoding.UTF8, MediaTypeNames.Application.Json);
            this._client.DefaultRequestHeaders.Accept.Remove(_mediaTypeHeaderValue);
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain));

            // Act
            var response = await this._client.PostAsync(c_apiUrl, requestContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        /// <summary>
        /// Agile story 1 test nr. 4.
        /// </summary>
        [Fact]
        public async void Post_CorrectBodyFormat_ReturnOkAndContent()
        {
            // Arrange
            var requestContent = new StringContent(c_jsonRequestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

            // Act
            var response = await this._client.PostAsync(c_apiUrl, requestContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<Response>(JsonSerializer.Deserialize<Response>(response.Content.ReadAsStream()));
        }

        /// <summary>
        /// Agile story 2 test nr. 1.
        /// </summary>
        [Theory]
        [MemberData(nameof(GetData))]
        public async void Post_MultipleInput_ReturnExpectedGisResults(
            string jsonRequestBody,
            List<GisResult> expectedGisResults,
            List<Activation> expectedActivations)
        {
            // Arrange
            var requestContent = new StringContent(jsonRequestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

            // Act
            var response = await this._client.PostAsync(c_apiUrl, requestContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Response? actualResponse = await response.Content.ReadFromJsonAsync<Response>(_JSON_SERIALIZER_OPTIONS);
            Assert.NotNull(actualResponse);

            Assert.True(Enumerable.SequenceEqual(
                actualResponse.GisResults.OrderBy(gr => gr.ToString()),
                expectedGisResults.OrderBy(gr => gr.ToString())));

            Assert.True(Enumerable.SequenceEqual(
                actualResponse.Activations.OrderBy(activation => activation.FachstellenId),
                expectedActivations.OrderBy(activation => activation.FachstellenId)));
        }


        /// <summary>
        /// Agile story 3 test nr. 1.
        /// </summary>
        [Fact]
        public async void LoadQueries_EmptyBody_ReturnsNoContent()
        {
            // Arrange
            var requestContent = new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json);

            // Act
            var response = await this._client.PostAsync(c_loadQueriesUrl, requestContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        public static IEnumerable<object[]>? GetData()
        {
            string jsonRequestBodyPolygon = "{\"shapes\":[\"POLYGON((2627973.6339951 1254304.548002," +
                "2627980.2419907 1254307.8530007,2627988.3290024 1254291.5920022,2627980.937999 1254288.1130044," +
                "2627975.0249985 1254299.6790017,2627975.9809922 1254299.7660012,2627973.6339951 1254304.548002))\"]," +
                "\"points\":[\"POINT(2627981.111998 1254296.9829995)\"],\"strings\":{\"baugesuch_prj_ort1\":\"Lausen\"}" +
                ",\"integers\":{\"baugesuch_gebauedetyp_id\":210}}";

            string jsonRequestBodyPoint = "{\"shapes\":[\"POINT(2613758.7035002 1260204.3868694)\"]," +
                "\"points\":[\"POINT(2613759.3435026 1260203.9068706)\"],\"strings\":{\"baugesuch_prj_ort1\":\"Lausen\"}" +
                ",\"integers\":{\"baugesuch_gebauedetyp_id\":210}}";

            string jsonRequestBodyMultiplePoints = "{\"shapes\":[\"POLYGON((2630979.2954982 1257775.1329996," +
                "2630980.9754909 1257774.9730021,2630980.2554958 1257771.2130045,2630978.8154933 1257771.6930033," +
                "2630979.2954982 1257775.1329996))\",\"LINESTRING(2630973.3754909 1257783.0530009," +
                "2630978.0954982 1257782.0130045,2630977.6954982 1257779.5329996)\"," +
                "\"LINESTRING(2630974.6554958 1257766.7329996,2630975.0554958 1257769.0530009)\"]," +
                "\"points\":[\"POINT(2630979.2954982 1257773.4530009)\",\"POINT(2630977.7754909 1257780.9730021)\"," +
                "\"POINT(2630974.8954982 1257768.0130045)\"]}";

            // Get the current date and 00:00:00 to compare the dates correctly.
            DateTime currentDate = DateTime.Today;
            string formattedDate = currentDate.ToString("dd.MM.yyyy 00:00:00");

            string gisResultPolygon = "[{\"queryResults\":[{\"queryId\":72,\"queryName\":\"Wildruhegebiet\",\"results\":{\"wrg\":[\"Wildruhegebiet\",\"Wildruhegebiet\"]}},{\"queryId\":5,\"queryName\":\"LIEGENSCHAFT\",\"results\":{\"flaechenmass\":[\"2706\"],\"egris_egrid\":[\"CH727001610832\"]}},{\"queryId\":36,\"queryName\":\"Strassenlaermbelastet\",\"results\":{\"slb\":[\"Strassenlärmbelastet\"],\"bemerkung\":[\"Gebiet mit Lärmbelastung durch Strassenlärm über Planungswert ES II, Stand LBK VT 2015\"],\"stand_datum\":[\"26.06.2019 13:57:25\"]}},{\"queryId\":18,\"queryName\":\"Mobilfunkantennen\",\"results\":{\"site_id\":[\"BL1047B\"],\"perimeter\":[\"56.7\"],\"distance\":[\"0\"]}},{\"queryId\":21,\"queryName\":\"Altlasten\",\"results\":{\"typ_bel\":[\"betriebsstandort, unbelastet\"]}},{\"queryId\":22,\"queryName\":\"Altlasten Parzelle\",\"results\":{\"typ_bel\":[\"betriebsstandort, unbelastet\"]}},{\"queryId\":29,\"queryName\":\"Gewässer\",\"results\":{\"gewaessername\":[\"Diegterbach\",\"Diegterbach\",\"Diegterbach\",\"Diegterbach\",\"Diegterbach\",\"Hefletenbächli\"],\"besitzer\":[\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\"],\"typ\":[\"Hauptgewässer\",\"Hauptgewässer\",\"Hauptgewässer\",\"Hauptgewässer\",\"Hauptgewässer\",\"Nebengewässer B\"],\"verlauf_typ\":[\"Bachlauf offen Hauptgewässer\",\"Bachlauf unter Brücke Hauptgewässer\",\"Bachlauf offen Hauptgewässer\",\"Bachlauf offen Hauptgewässer\",\"Bachlauf offen Hauptgewässer\",\"Bachlauf eingedolt Nebengewässer B\"],\"distance\":[\"13\",\"39\",\"46\",\"53\",\"54\",\"77\"]}},{\"queryId\":2,\"queryName\":\"ZONE\",\"results\":{\"kant_bezeichnung\":[\"G2\"],\"typ_bezeichnung\":[\"Gewerbezone G\"],\"planinhalt\":[\"verbindlich\"]}},{\"queryId\":7,\"queryName\":\"NP_ZONENPLAN\",\"results\":{\"rechtsstatus\":[\"in Kraft\"],\"genehmigung_nr\":[\"772\"],\"genehmigung_datum\":[\"04.06.2019 00:00:00\"],\"plan_nr\":[\"64/ZPS/1/6\"]}},{\"queryId\":25,\"queryName\":\"GW-Fassung\",\"results\":{\"klnr\":[\"73.J.10\",\"73.J.8\",\"73.J.9\"]}},{\"queryId\":4,\"queryName\":\"ZPL_ZPS\",\"results\":{\"bg_typ\":[\"innerhalb ZPS -> RBG\"]}},{\"queryId\":61,\"queryName\":\"GW-Mittelwasser\",\"results\":{\"hoehe\":[\"417 m ü.M.\"]}},{\"queryId\":0,\"queryName\":\"Datum\",\"results\":{\"date\":[\"" + formattedDate + "\"]}},{\"queryId\":16,\"queryName\":\"GW-Gewaesserschutzbereich\",\"results\":{\"art\":[\"Gewässerschutzbereich Au (unterirdisch)\"]}},{\"queryId\":3,\"queryName\":\"Denkmalschutz-Objekt\",\"results\":{\"objektname\":[\"Ev.-ref. Pfarrhaus mit  Brunnen\",\"Ev.-ref. Pfarrhaus mit  Brunnen\",\"Ev.-ref. Kirche\",\"Ev.-ref. Kirche\",\"Pfarrscheune\",\"Ev.-ref. Kirche\",\"Joggi-Mohler Brüggli\",\"Heuschürli\",\"Getreidespeicher\",\"Speicher\",\"Zehntenhaus\"],\"distance\":[\"331\",\"339\",\"346\",\"359\",\"360\",\"373\",\"999\",\"1115\",\"1164\",\"1183\",\"1192\"]}},{\"queryId\":41,\"queryName\":\"LZE-NL-ZPS_Landwirtschaftszone\",\"results\":{\"wert\":[\"grenzt an ZPL\"]}}]},{\"queryResults\":[{\"queryId\":202,\"queryName\":\"BIT-NG-Naturgefahren\",\"results\":{\"resvalue\":[\"W mittel\"]}},{\"queryId\":201,\"queryName\":\"BGV-ESP-Naturgefahren\",\"results\":{\"resvalue\":[\"W gering, Oberflächenabfluss, W mittel\"]}}]}]";
            string gisResultPoint = "[{\"queryResults\":[{\"queryId\":23,\"queryName\":\"StFV\",\"results\":{\"firma\":[\"Swisscom Arlesheim\",\"Weleda AG\",\"Felix Transport AG\",\"Stöcklin Logistik AG\",\"Schwimmbad Reinach\"],\"distance\":[\"859\",\"1086\",\"1207\",\"1266\",\"1460\"]}},{\"queryId\":17,\"queryName\":\"Wärmeverbund\",\"results\":{\"waermeverbund\":[\"WVB Arlesheim Domplatz\"]}},{\"queryId\":5,\"queryName\":\"LIEGENSCHAFT\",\"results\":{\"flaechenmass\":[\"4891\"],\"egris_egrid\":[\"CH527708824935\"]}},{\"queryId\":9,\"queryName\":\"Denkmalschutz Ortsbild\",\"results\":{\"case\":[\"innerhalb\"]}},{\"queryId\":19,\"queryName\":\"Teilzonenplan, Quartierplan\",\"results\":{\"zweckbestimmung\":[\"QP Ortskern\"],\"genehmigung_nr\":[\"571\"],\"genehmigung_datum\":[\"02.05.2017 00:00:00\"],\"plan_nr\":[\"06/QP/2/24\"]}},{\"queryId\":22,\"queryName\":\"Altlasten Parzelle\",\"results\":{\"typ_bel\":[\"betriebsstandort, unbelastet\"]}},{\"queryId\":29,\"queryName\":\"Gewässer\",\"results\":{\"gewaessername\":[\"Dorfbach\"],\"besitzer\":[\"öffentliches Gewässer\"],\"typ\":[\"Nebengewässer A\"],\"verlauf_typ\":[\"Bachlauf eingedolt Nebengewässer A\"],\"distance\":[\"28\"]}},{\"queryId\":2,\"queryName\":\"ZONE\",\"results\":{\"kant_bezeichnung\":[\"Perimeter Sondernutzungsplan\"],\"typ_bezeichnung\":[\"Perimeter Sondernutzungsplan\"],\"planinhalt\":[\"verbindlich\"]}},{\"queryId\":33,\"queryName\":\"Denkmalschutz ISOS A\",\"results\":{\"isosa\":[\"betroffen\"]}},{\"queryId\":7,\"queryName\":\"NP_ZONENPLAN\",\"results\":{\"rechtsstatus\":[\"in Kraft\",\"in Kraft\"],\"genehmigung_nr\":[\"13\",\"571\"],\"genehmigung_datum\":[\"10.01.2023 00:00:00\",\"02.05.2017 00:00:00\"],\"plan_nr\":[\"06/ZPS/3/8\",\"06/QP/2/24\"]}},{\"queryId\":8,\"queryName\":\"Denkmalschutz Nachbarparzelle\",\"results\":{\"objektname\":[\"Domkirche\"],\"parzellen\":[\"87\"],\"nummer\":[\"87\"]}},{\"queryId\":0,\"queryName\":\"Datum\",\"results\":{\"date\":[\"" + formattedDate + "\"]}},{\"queryId\":4,\"queryName\":\"ZPL_ZPS\",\"results\":{\"bg_typ\":[\"innerhalb ZPS -> RBG\"]}},{\"queryId\":3,\"queryName\":\"Denkmalschutz-Objekt\",\"results\":{\"objektname\":[\"Bezirksstatthalteramt (ehem. Domherrenhaus)\",\"Bezirksschreiberei (ehem. Domherrenhaus)\",\"Bezirksschreiberei (ehem. Domherrenhaus)\",\"Domkirche\",\"Bezirksgericht (ehem. Domherrenhaus)\",\"Pfarreizentrum (Domhof, ehem. Domherrenhaus)\",\"Bezirksgericht (ehem. Domherrenhaus)\",\"Röm.-kath. Pfarramt (ehem. Domherrenhaus)\",\"Gemeindeverwaltung (ehem. Domherrenhaus)\",\"Wohnhaus\",\"Gartenpavillon\",\"Ehem. Pfarrhaus\",\"Hofgut Arlesheim beim Andlauerhof\",\"Museum Trotte\",\"Scheune des Hofgutes Arlesheim\",\"Domplatzschulhaus\",\"Fallerhof\",\"Hofgut Arlesheim beim Andlauerhof\",\"Ermitagestrasse\",\"Hofgut Arlesheim beim Andlauerhof\",\"Sundgauerhof\",\"Wohnhaus\",\"Schleife\",\"Villa Visscher van Gaasbeek\",\"Ev.-ref. Kirche\",\"Gärtnerhaus\",\"Landschaftsgarten Ermitage\",\"Mühle\",\"Landschaftsgarten Ermitage\",\"Schloss Birseck\",\"Mühle\",\"Landschaftsgarten Ermitage\",\"Landschaftsgarten Ermitage\",\"Landschaftsgarten Ermitage\",\"Schopf Weidhof\",\"Scheune Weidhof\",\"Taubenhaus Weidhof\",\"Schaffnerhaus\",\"Landschaftsgarten Ermitage\",\"Landschaftsgarten Ermitage\",\"Burg Reichenstein\",\"Büchsenschmiede\",\"Waldhaus\",\"Büchsenschmiede\",\"Landschaftsgarten Ermitage\"],\"distance\":[\"14\",\"25\",\"29\",\"40\",\"59\",\"67\",\"69\",\"73\",\"85\",\"96\",\"120\",\"124\",\"126\",\"127\",\"141\",\"143\",\"163\",\"163\",\"166\",\"174\",\"218\",\"235\",\"306\",\"460\",\"482\",\"504\",\"516\",\"517\",\"518\",\"527\",\"536\",\"541\",\"543\",\"545\",\"554\",\"570\",\"587\",\"593\",\"673\",\"739\",\"752\",\"907\",\"910\",\"932\",\"982\"]}}]}]";
            string gisResultMultiplePoints = "[{\"queryResults\":[{\"queryId\":29,\"queryName\":\"Gewässer\",\"results\":{\"gewaessername\":[\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\"],\"besitzer\":[\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\"],\"typ\":[\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\"],\"verlauf_typ\":[\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf offen Nebengewässer A\"],\"distance\":[\"84\",\"84\",\"85\",\"85\",\"86\",\"88\",\"92\",\"92\",\"95\"]}},{\"queryId\":2,\"queryName\":\"ZONE\",\"results\":{\"kant_bezeichnung\":[\"W2\"],\"typ_bezeichnung\":[\"Wohnzone W\"],\"planinhalt\":[\"verbindlich\"]}},{\"queryId\":23,\"queryName\":\"StFV\",\"results\":{\"firma\":[\"Synco Chemie AG / Avellis Synco Leather (Swiss) AG\"],\"distance\":[\"1122\"]}},{\"queryId\":7,\"queryName\":\"NP_ZONENPLAN\",\"results\":{\"rechtsstatus\":[\"in Kraft\"],\"genehmigung_nr\":[\"1429\"],\"genehmigung_datum\":[\"19.10.2021 00:00:00\"],\"plan_nr\":[\"25/ZPS/3/5\"]}},{\"queryId\":4,\"queryName\":\"ZPL_ZPS\",\"results\":{\"bg_typ\":[\"innerhalb ZPS -> RBG\"]}},{\"queryId\":5,\"queryName\":\"LIEGENSCHAFT\",\"results\":{\"flaechenmass\":[\"334\"],\"egris_egrid\":[\"CH980361087959\"]}},{\"queryId\":0,\"queryName\":\"Datum\",\"results\":{\"date\":[\"" + formattedDate + "\"]}},{\"queryId\":3,\"queryName\":\"Denkmalschutz-Objekt\",\"results\":{\"objektname\":[\"Pümpin Haus\",\"s¿ Balle-Phiggi¿s Huus\",\"Völlmi Sämi Huus\",\"Restaurant Bären\",\"Gerichtsgebäude\",\"Dorfbrunnen\",\"Bürgerhaus\",\"Wohnhaus\",\"Müller-Jörke-Haus\",\"Wohn-, Geschäftshaus\",\"Oekonomiegebäude\",\"Bauernhaus\",\"Bauernhaus\",\"Pfarrhaus\",\"Ev.-ref. Kirche\",\"Ev.-ref. Kirche\",\"Ev.-ref. Kirche\",\"Jundt-Huus\",\"Bauernhaus\",\"Bauernhaus\",\"Bauernhaus\",\"Gartenhaus\",\"Wohnhaus (ehem. Zehntenscheune)\",\"Friedhofkapelle\",\"Heuschürli\",\"Wohnhaus\"],\"distance\":[\"434\",\"664\",\"681\",\"686\",\"781\",\"790\",\"807\",\"845\",\"846\",\"869\",\"883\",\"892\",\"902\",\"912\",\"936\",\"944\",\"952\",\"968\",\"969\",\"974\",\"981\",\"1167\",\"1213\",\"1226\",\"1227\",\"1293\"]}}]},{\"queryResults\":[{\"queryId\":23,\"queryName\":\"StFV\",\"results\":{\"firma\":[\"Synco Chemie AG / Avellis Synco Leather (Swiss) AG\"],\"distance\":[\"1123\"]}},{\"queryId\":4,\"queryName\":\"ZPL_ZPS\",\"results\":{\"bg_typ\":[\"innerhalb ZPS -> RBG\"]}},{\"queryId\":29,\"queryName\":\"Gewässer\",\"results\":{\"gewaessername\":[\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\"],\"besitzer\":[\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\"],\"typ\":[\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\"],\"verlauf_typ\":[\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf offen Nebengewässer A\"],\"distance\":[\"84\",\"84\",\"85\",\"85\",\"86\",\"88\",\"92\",\"92\",\"95\"]}},{\"queryId\":2,\"queryName\":\"ZONE\",\"results\":{\"kant_bezeichnung\":[\"W2\"],\"typ_bezeichnung\":[\"Wohnzone W\"],\"planinhalt\":[\"verbindlich\"]}},{\"queryId\":7,\"queryName\":\"NP_ZONENPLAN\",\"results\":{\"rechtsstatus\":[\"in Kraft\"],\"genehmigung_nr\":[\"1429\"],\"genehmigung_datum\":[\"19.10.2021 00:00:00\"],\"plan_nr\":[\"25/ZPS/3/5\"]}},{\"queryId\":5,\"queryName\":\"LIEGENSCHAFT\",\"results\":{\"flaechenmass\":[\"334\"],\"egris_egrid\":[\"CH980361087959\"]}},{\"queryId\":0,\"queryName\":\"Datum\",\"results\":{\"date\":[\"" + formattedDate + "\"]}},{\"queryId\":3,\"queryName\":\"Denkmalschutz-Objekt\",\"results\":{\"objektname\":[\"Pümpin Haus\",\"s¿ Balle-Phiggi¿s Huus\",\"Völlmi Sämi Huus\",\"Restaurant Bären\",\"Gerichtsgebäude\",\"Dorfbrunnen\",\"Bürgerhaus\",\"Wohnhaus\",\"Müller-Jörke-Haus\",\"Wohn-, Geschäftshaus\",\"Oekonomiegebäude\",\"Bauernhaus\",\"Bauernhaus\",\"Pfarrhaus\",\"Ev.-ref. Kirche\",\"Ev.-ref. Kirche\",\"Ev.-ref. Kirche\",\"Jundt-Huus\",\"Bauernhaus\",\"Bauernhaus\",\"Bauernhaus\",\"Gartenhaus\",\"Wohnhaus (ehem. Zehntenscheune)\",\"Friedhofkapelle\",\"Heuschürli\",\"Wohnhaus\"],\"distance\":[\"442\",\"670\",\"687\",\"692\",\"787\",\"796\",\"814\",\"852\",\"853\",\"876\",\"889\",\"898\",\"908\",\"918\",\"942\",\"950\",\"958\",\"975\",\"975\",\"980\",\"988\",\"1171\",\"1216\",\"1219\",\"1225\",\"1296\"]}}]},{\"queryResults\":[{\"queryId\":29,\"queryName\":\"Gewässer\",\"results\":{\"gewaessername\":[\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\",\"Rickenbächli\"],\"besitzer\":[\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\",\"öffentliches Gewässer\"],\"typ\":[\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\",\"Nebengewässer A\"],\"verlauf_typ\":[\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf eingedolt Nebengewässer A\",\"Bachlauf offen Nebengewässer A\",\"Bachlauf offen Nebengewässer A\"],\"distance\":[\"84\",\"84\",\"85\",\"85\",\"86\",\"88\",\"92\",\"92\",\"95\"]}},{\"queryId\":2,\"queryName\":\"ZONE\",\"results\":{\"kant_bezeichnung\":[\"W2\"],\"typ_bezeichnung\":[\"Wohnzone W\"],\"planinhalt\":[\"verbindlich\"]}},{\"queryId\":23,\"queryName\":\"StFV\",\"results\":{\"firma\":[\"Synco Chemie AG / Avellis Synco Leather (Swiss) AG\"],\"distance\":[\"1126\"]}},{\"queryId\":7,\"queryName\":\"NP_ZONENPLAN\",\"results\":{\"rechtsstatus\":[\"in Kraft\"],\"genehmigung_nr\":[\"1429\"],\"genehmigung_datum\":[\"19.10.2021 00:00:00\"],\"plan_nr\":[\"25/ZPS/3/5\"]}},{\"queryId\":4,\"queryName\":\"ZPL_ZPS\",\"results\":{\"bg_typ\":[\"innerhalb ZPS -> RBG\"]}},{\"queryId\":5,\"queryName\":\"LIEGENSCHAFT\",\"results\":{\"flaechenmass\":[\"334\"],\"egris_egrid\":[\"CH980361087959\"]}},{\"queryId\":0,\"queryName\":\"Datum\",\"results\":{\"date\":[\"" + formattedDate + "\"]}},{\"queryId\":3,\"queryName\":\"Denkmalschutz-Objekt\",\"results\":{\"objektname\":[\"Pümpin Haus\",\"s¿ Balle-Phiggi¿s Huus\",\"Völlmi Sämi Huus\",\"Restaurant Bären\",\"Gerichtsgebäude\",\"Dorfbrunnen\",\"Bürgerhaus\",\"Müller-Jörke-Haus\",\"Wohnhaus\",\"Wohn-, Geschäftshaus\",\"Oekonomiegebäude\",\"Bauernhaus\",\"Bauernhaus\",\"Pfarrhaus\",\"Ev.-ref. Kirche\",\"Ev.-ref. Kirche\",\"Ev.-ref. Kirche\",\"Jundt-Huus\",\"Bauernhaus\",\"Bauernhaus\",\"Bauernhaus\",\"Gartenhaus\",\"Wohnhaus (ehem. Zehntenscheune)\",\"Friedhofkapelle\",\"Heuschürli\",\"Wohnhaus\"],\"distance\":[\"431\",\"664\",\"681\",\"686\",\"781\",\"789\",\"807\",\"844\",\"845\",\"869\",\"882\",\"891\",\"902\",\"911\",\"936\",\"943\",\"951\",\"967\",\"968\",\"973\",\"980\",\"1160\",\"1206\",\"1232\",\"1234\",\"1286\"]}}]},{\"queryResults\":[{\"queryId\":201,\"queryName\":\"BGV-ESP-Naturgefahren\",\"results\":{\"resvalue\":[\"Oberflächenabfluss\"]}}]}]";

            List<GisResult>? gisResultsPolygon = JsonSerializer.Deserialize<List<GisResult>>(
                gisResultPolygon,
                _JSON_SERIALIZER_OPTIONS);

            List<GisResult>? gisResultsPoint = JsonSerializer.Deserialize<List<GisResult>>(
                gisResultPoint,
                _JSON_SERIALIZER_OPTIONS);

            List<GisResult>? gisResultsMultiplePoints = JsonSerializer.Deserialize<List<GisResult>>(
                gisResultMultiplePoints,
                _JSON_SERIALIZER_OPTIONS);

            string activationPolygon = "[{\"fachstellenId\":305,\"activationRemark\":\"grenzt an ZPL\"},{\"fachstellenId\":307,\"activationRemark\":\"Strassenlärmbelastet\"},{\"fachstellenId\":333,\"activationRemark\":\"Altlast Parzelle: betriebsstandort, unbelastet\"},{\"fachstellenId\":334,\"activationRemark\":\"GSB=Gewässerschutzbereich Au (unterirdisch)\"},{\"fachstellenId\":369,\"activationRemark\":\"GSB=Gewässerschutzbereich Au (unterirdisch), GWA=73.J.10, 73.J.8, 73.J.9, GWM=417 m ü.M., Altlast=betriebsstandort, unbelastet\"},{\"fachstellenId\":370,\"activationRemark\":\"GWA=73.J.10, 73.J.8, 73.J.9\"},{\"fachstellenId\":371,\"activationRemark\":\"Altlast Parzelle: betriebsstandort, unbelastet\"},{\"fachstellenId\":372,\"activationRemark\":\"Altlast: betriebsstandort, unbelastet, Altlast Parzelle: betriebsstandort, unbelastet\"},{\"fachstellenId\":405,\"activationRemark\":\"innerh. Strahlungsperimeter: BL1047B\"},{\"fachstellenId\":425,\"activationRemark\":\"W gering, Oberflächenabfluss, W mittel\"},{\"fachstellenId\":434,\"activationRemark\":\"W mittel\"},{\"fachstellenId\":437,\"activationRemark\":\"Wildruhegebiet\"}]";
            string activationPoint = "[{\"fachstellenId\":306,\"activationRemark\":\"Distanz: 14, 25, 29, 40, 59, 67, 69, 73, 85, 96, 120, 124, 126, 127, 141, 143, 163, 163, 166, 174, 218, 235, 306, 460, 482, 504, 516, 517, 518, 527, 536, 541, 543, 545, 554, 570, 587, 593, 673, 739, 752, 907, 910, 932, 982, NachbarGst: 87, ISOS A Prüfung Solaranlage: betroffen\"},{\"fachstellenId\":333,\"activationRemark\":\"Altlast Parzelle: betriebsstandort, unbelastet\"},{\"fachstellenId\":335,\"activationRemark\":\"innerhalb Wärmeverbund, QP Ortskern\"},{\"fachstellenId\":369,\"activationRemark\":\"Altlast=betriebsstandort, unbelastet\"},{\"fachstellenId\":371,\"activationRemark\":\"Altlast Parzelle: betriebsstandort, unbelastet\"},{\"fachstellenId\":372,\"activationRemark\":\"Altlast Parzelle: betriebsstandort, unbelastet\"}]";
            string activationMultiplePoints = "[{\"fachstellenId\":425,\"activationRemark\":\"Oberflächenabfluss\"}]";

            List<Activation>? activationsPolygon = JsonSerializer.Deserialize<List<Activation>>(
                activationPolygon,
                _JSON_SERIALIZER_OPTIONS);

            List<Activation>? activationsPoint = JsonSerializer.Deserialize<List<Activation>>(
                activationPoint,
                _JSON_SERIALIZER_OPTIONS);

            List<Activation>? activationsPoints = JsonSerializer.Deserialize<List<Activation>>(
                activationMultiplePoints,
                _JSON_SERIALIZER_OPTIONS);

            if (gisResultsPolygon is null || 
                activationsPolygon is null || 
                gisResultsPoint is null || 
                activationsPoint is null || 
                gisResultsMultiplePoints is null || 
                activationsPoints is null)
            {
                return null;
            }

            var allData = new List<object[]>
            {
                new object[] { jsonRequestBodyPolygon, gisResultsPolygon, activationsPolygon },
                new object[] { jsonRequestBodyPoint, gisResultsPoint, activationsPoint },
                new object[] { jsonRequestBodyMultiplePoints, gisResultsMultiplePoints, activationsPoints },
            };

            return allData;
        }
    }
}