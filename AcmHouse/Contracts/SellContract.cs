using AcmHouse.AccommodationInfo;
using AcmHouse.CompanyInfo;
using AcmHouse.Enums;
using AcmHouse.LandlordInfo;
using AcmHouse.SaleAccommodationsInfo;
using AcmHouse.SellersInfo;
using AcmHouse.UsersInfo;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Windows;

namespace AcmHouse.Contracts
{
    public class SellContract
    {
        public CurrentUser currentUserData = CurrentUserManager.CurrentUser;
        private Seller seller;
        CompanyInfo.Company company = new CompanyInfo.Company();
        public Seller Seller
        {
            get { return seller; }
            set { seller = value; }
        }

        public SellContract(Seller seller)
        {
            Seller = seller;
        }
        public void GenerateRealEstateContract(string outputPath, Seller seller, CurrentUser currentUser, SaleAccommodation accommodation, int ordersCounter, DateTime startDate, DateTime endDate)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();

                Body body = mainPart.Document.AppendChild(new Body());

                // Add title of the contract
                Paragraph titleParagraph = CreateParagraph("\t\t\t\tДОГОВОР\r\nна оказание риэлтерских услуг продавцу объекта недвижимости", "");
                body.AppendChild(titleParagraph);

                // Main text of the contract
                string contractText = $"{seller.Surname} {seller.Initials}, в дальнейшем именуемый «Потребитель», и {company.Name} на основании:\r\n" +
                    $"лицензии Министерства юстиции Республики Беларусь, \r\n" +
                    $"договора обязательного страхования ответственности за причинение вреда в связи с осуществлением риэлтерской деятельности,\r\n" +
                    $"страховой полис, в лице директора {company.OwnerInitials}, действующего на основании Устава, в дальнейшем именуемое «Исполнитель»,\r\n" +
                    $"совместно именуемые «Стороны», заключили настоящий Договор о нижеследующем:";
                body.AppendChild(CreateParagraph(contractText, "\n"));

                // Subject of the contract
                body.AppendChild(CreateParagraph("1. ПРЕДМЕТ ДОГОВОРА", "\n", true));
                string subjectText = $"1.1. Предметом настоящего Договора является оказание Исполнителем Потребителю риэлтерских услуг (далее- услуги) по отчуждению принадлежащего Потребителю на праве собственности объекта недвижимости, представляющего собой {accommodation.AccommodationType}\r\n" +
                    $"по адресу: {accommodation.Adress} (далее - объект недвижимости). Описание объекта недвижимости приведено в приложении N° 1 к настоящему Договору, являющимся неотъемлемой частью настоящего Договора.\r\n" +
                    "1.2. Исполнитель оказывает следующие услуги (нужное отметить):\r\n" +
                    "1.2.1. на этапе подбора варианта сделки с объектом недвижимости (поиск покупателя)\r\n" +
                    "- консультационные услуги по сделкам с объектами недвижимости (далее- сделка):\r\n" +
                    "- предоставление информации о спросе и предложении на объекты недвижимости;\r\n" +
                    "- организация и размещение рекламы объекта недвижимости;\r\n" +
                    "-осмотр объекта недвижимости потенциальными покупателями;\r\n" +
                    "о подбор вариантов сделки с объектом недвижимости.\r\n" +
                    "1.2.2. на этапе отчуждения объекта недвижимости (при подобранном варианте сделки)\r\n" +
                    "- консультационные услуги по сделке с объектом недвижимости;\r\n" +
                    "- организация и проведение согласования условий предстоящей сделки;\r\n" +
                    "- помощь в подготовке (оформлении) документов, связанных с осуществлением сделки, и содействие в нотариальном удостоверении сделки с объектом недвижимости;\r\n" +
                    "- содействие в государственной регистрации сделки с объектом недвижимости.";
                body.AppendChild(CreateSingleParagraph(subjectText));

                // Obligations of the parties at the stage of selecting a transaction option
                body.AppendChild(CreateParagraph("2. ОБЯЗАННОСТИ СТОРОН НА ЭТАПЕ ПОДБОРА ВАРИАНТА СДЕЛКИ", "\n", true));
                string obligationsText = "2.1. Исполнитель обязуется:\r\n" +
                    "2.1.1. Предоставить Потребителю консультации по правовым, организационным и иным вопросам, касающимся отчуждения объекта недвижимости.\r\n" +
                    "2.1.2. Организовать поиск потенциального покупателя объекта недвижимости:\r\n" +
                    "разместить информацию об объекте недвижимости в базе данных Исполнителя в течение 1 дня;\r\n" +
                    "организовать рекламу объекта недвижимости в специализированных средствах массовой информации.\r\n" +
                    "2.1.3. Организовать осмотры объекта недвижимости потенциальными покупателями по предварительному согласованию с Потребителем, осуществляемые в сопровождении представителя Исполнителя.\r\n" +
                    "2.2. Потребитель обязуется:\r\n" +
                    "2.2.1. Все действия, охватываемые предметом настоящего Договора, осуществлять в присутствии представителей Исполнителя. По требованию Исполнителя предоставлять возможность проведения осмотров отчуждаемого объекта недвижимости потенциальному покупателю.\r\n" +
                    "2.2.2. Передать Исполнителю на хранение необходимые для исполнения настоящего Договора документы на объект недвижимости. Перечень переданных Потребителем Исполнителю документов содержится в приложении No 4 к настоящему Договору.\r\n" +
                    "2.2.3. Не совершать действий, препятствующих исполнению Договора:\r\n" +
                    "не использовать объект недвижимости в качестве залога;\r\n" +
                    "не отчуждать объект недвижимости без участия Исполнителя и т.д.\r\n" +
                    "2.2.4. Присутствовать при совершении действий, требующих его участия.\r\n" +
                    "2.2.5. В письменном виде сообщать Исполнителю о возникновении всех обстоятельств, которые могут повлиять на исполнение или надлежащее исполнение настоящего Договора, в том числе об изменении стоимости, по которой объект недвижимости выставлен на продажу.\r\n" +
                    "2.2.6. Известить Исполнителя в письменной форме о намерении расторгнуть или изменить условия настоящего Договора;\r\n" +
                    "2.2.7. Оплатить услуги Исполнителя в соответствии с условиями настоящего Договора.";
                body.AppendChild(CreateSingleParagraph(obligationsText));

                // Obligations of the parties at the stage of selling real estate
                body.AppendChild(CreateParagraph("3. ОБЯЗАННОСТИ СТОРОН НА ЭТАПЕ ПРОДАЖИ ОБЪЕКТА НЕДВИЖИМОСТИ", "\n", true));
                string saleObligationsText = "3.1. Исполнитель обязуется:\r\n" +
                    "3.1.1. Предоставить Потребителю консультации о порядке совершения сделки и о необходимых для этого документах. Организовать переговоры между Потребителем и покупателем об условиях предстоящей сделки;\r\n" +
                    "3.1.2. Организовать согласование условий предстоящей сделки с объектом недвижимости между Потребителем и потенциальным покупателем объекта недвижимости в письменном виде;\r\n" +
                    "3.1.3. Организовать сбор документов, необходимых для совершения сделки, и государственной регистрации сделки и государственной регистрации перехода права собственности;\r\n" +
                    "3.1.4. При необходимости действовать от имени Потребителя по доверенности.\r\n" +
                    "3.1.5. Для удостоверения договора и регистрации сделки, предварительно записаться на прием к нотариусу (регистратору АГР и ЗК), в назначенное нотариусом (регистратором АГР и ЗК) время предоставить все документы, необходимые для совершения отчуждения объекта недвижимости и его государственной регистрации;\r\n" +
                    "3.1.6. Информировать Потребителя о ходе исполнения настоящего Договора. Незамедлительно предупреждать о возможных препятствиях и изменениях в процессе совершения сделки.\r\n" +
                    "3.1.7. Содействовать по инициативе Потребителя организации взаиморасчетов между сторонами сделки с объектом недвижимости;\r\n" +
                    "3.1.8. Проверить правильность оформления и комплектность правоустанавливающих документов на объект недвижимости;\r\n" +
                    "3.1.9. Обеспечить сохранность документов, принятых от Потребителя;\r\n" +
                    "3.1.10. Обеспечить конфиденциальность информации, полученной в процессе предоставления услуг Потребителю;\r\n" +
                    "3.1.11. В случае возникновения спора о действительности сделки отстаивать интересы Потребителя в соответствующих органах и организациях, в том числе в суде в соответствии с законодательством.";
                body.AppendChild(CreateSingleParagraph(saleObligationsText));

                // Cost of services and payment procedure
                body.AppendChild(CreateParagraph("4. СТОИМОСТЬ УСЛУГ И ПОРЯДОК РАСЧЕТОВ", "\n", true));
                string paymentText = "4.1. Стоимость услуг Исполнителя определяется в процентном отношении от цены объекта недвижимости, указанной в договоре купли-продажи (мены и др.) или предварительном договоре (протоколе о намерениях) и составляет __%.\r\n" +
                    "4.2. Оплата услуг Исполнителя осуществляется Потребителем по факту их выполнения путем перечисления денежных средств на расчетный счет Исполнителя или внесения наличных в кассу Исполнителя.\r\n" +
                    "4.3. Потребитель оплачивает стоимость рекламных услуг Исполнителя.";
                body.AppendChild(CreateSingleParagraph(paymentText));

                // Responsibility of the parties
                body.AppendChild(CreateParagraph("5. ОТВЕТСТВЕННОСТЬ СТОРОН", "\n", true));
                string liabilityText = "5.1. Исполнитель несет ответственность за качество оказываемых услуг и их соответствие условиям настоящего Договора.\r\n" +
                    "5.2. Потребитель несет ответственность за достоверность и полноту предоставленной Исполнителю информации, а также за своевременность выполнения своих обязательств по настоящему Договору.";
                body.AppendChild(CreateSingleParagraph(liabilityText));

                // Final provisions
                body.AppendChild(CreateParagraph("6. ЗАКЛЮЧИТЕЛЬНЫЕ ПОЛОЖЕНИЯ", "\n", true));
                string finalText = "6.1. Настоящий Договор вступает в силу с момента его подписания Сторонами и действует до полного исполнения обязательств.\r\n" +
                    "6.2. Все изменения и дополнения к настоящему Договору оформляются в письменной форме и подписываются Сторонами.\r\n" +
                    "6.3. Во всем остальном, что не предусмотрено настоящим Договором, Стороны руководствуются законодательством Республики Беларусь.";
                body.AppendChild(CreateSingleParagraph(finalText));

                // Signatures of the parties
                body.AppendChild(CreateParagraph("\tПодписи Сторон:", "\n", true));
                string signatures = $"Потребитель:\t\t\t\t\tИсполнитель:\r\n{seller.Surname} {seller.Initials}\t\t\t{currentUser.Surname} {currentUser.Name}\r\n________________________\t\t\t\t\t________________________";
                body.AppendChild(CreateSingleParagraph(signatures));

                // Requisites of the parties
                body.AppendChild(CreateParagraph("8. РЕКВИЗИТЫ И ПОДПИСИ СТОРОН", "\n", true));
                string requisitesText = $"Потребитель: {seller.Email}, тел. {seller.Phone}\r\n" +
                    $"Паспорт: {seller.PassportNumber}\r\n" +
                    $"(подпись) __________________________ (ФИО) {seller.Surname} {seller.Initials}\r\n" +
                    $"\r\nИсполнитель: {company.Name}, {company.Address}\r\n" +
                    $"р/с {company.AccountNumber} в {company.BankName},\r\n" +
                    $"ИНН {company.INN}, БИК {company.BIK}, Телефон/факс {company.Phone}\r\n" +
                    $"Ответственный по настоящему Договору риэлтер: {currentUser.Surname} {currentUser.Name}\r\n" +
                    $"(подпись) __________________________";
                body.AppendChild(CreateSingleParagraph(requisitesText));

                body.AppendChild(CreateParagraph(" ", " ")); // Empty paragraph

                // Table with information about the property and participants
                Table table = new Table();

                // Setting table borders
                TableProperties tblProps = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }));

                table.AppendChild(tblProps);

                // Method to create a table row
                TableRow CreateTableRow(string cell1, string cell2)
                {
                    TableRow tr = new TableRow();
                    TableCell tc1 = new TableCell(new Paragraph(new Run(new Text(cell1))));
                    TableCell tc2 = new TableCell(new Paragraph(new Run(new Text(cell2))));
                    tr.Append(tc1, tc2);
                    return tr;
                }

                // Adding rows to the table
                table.Append(
                    CreateTableRow("Сведения об объекте договора", ""),
                    CreateTableRow("Место нахождения недвижимого имущества:", accommodation.Adress),
                    CreateTableRow("Адрес:", accommodation.Adress),
                    CreateTableRow("Площадь:", accommodation.Area.ToString()),
                    CreateTableRow("Количество помешений:", accommodation.AmountOfRooms.ToString()),
                    CreateTableRow("Срок действия договора", ""),
                    CreateTableRow("Начало:", startDate.ToString("dd.MM.yyyy")),
                    CreateTableRow("Окончание:", endDate.ToString("dd.MM.yyyy")),
                    CreateTableRow("Сведения о риелторе:", ""),
                    CreateTableRow("ФИО", $"{currentUser.Surname} {currentUser.Name}"),
                    CreateTableRow("Число, месяц, год рождения", ""),
                    CreateTableRow("Идентификационный номер", $"{currentUser.IdentificationNumber}"),
                    CreateTableRow("Моб. номер", ""),
                    CreateTableRow("Почта", $"{currentUser.Email}"),
                    CreateTableRow("Сведения о продавце:", ""),
                    CreateTableRow("ФИО", $"{seller.Surname} {seller.Initials}"),
                    CreateTableRow("Число, месяц, год рождения", ""),
                    CreateTableRow("Идентификационный номер", seller.IdentificationNumber),
                    CreateTableRow("Моб. номер", $"{seller.Phone}"),
                    CreateTableRow("Почта", $"{seller.Email}"),
                    CreateTableRow("Подпись", ""),
                    CreateTableRow("Должность", ""));

                body.Append(table);

                // Save changes and close the document
                document.Save();

                mainPart.Document.Save();
            }
        }

        public void GenerateSaleContract(string outputPath, Seller seller, CurrentUser currentUser, Landlord buyer, SaleAccommodation accommodation, int ordersCounter, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (WordprocessingDocument document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = document.AddMainDocumentPart();
                    mainPart.Document = new Document();

                    Body body = mainPart.Document.AppendChild(new Body());

                    // Создаем заголовок договора
                    Paragraph titleParagraph = CreateParagraph($"ДОГОВОР № {ordersCounter} КУПЛИ-ПРОДАЖИ недвижимого имущества, находящегося в собственности Республики Беларусь", JustificationValues.Center, true);
                    body.AppendChild(titleParagraph);

                    // Место и дата заключения договора
                    Paragraph locationDateParagraph = CreateParagraph($"г. Минск\n«{DateTime.Today:dd.MM.yyyy}»", JustificationValues.Left, true);
                    body.AppendChild(locationDateParagraph);

                    // Введение сторон
                    string partiesText = $"{seller.Surname} {seller.Initials}, именуемый в дальнейшем «Продавец», в лице владельца имущества с одной стороны, и {buyer.Surname} {buyer.Name}, именуемый в дальнейшем «Покупатель» с другой стороны, заключили настоящий договор о нижеследующем:";
                    body.AppendChild(CreateParagraph(partiesText, JustificationValues.Both));

                    // Предмет договора
                    body.AppendChild(CreateParagraph("1. ПРЕДМЕТ ДОГОВОРА", JustificationValues.Left, true));
                    string subjectText = $"1.1. Продавец обязуется передать в собственность (хозяйственное ведение или оперативное управление) Покупателя {accommodation.Name}, расположенное по адресу: {accommodation.Adress} (далее – недвижимое имущество), а Покупатель обязуется принять в собственность (хозяйственное ведение или оперативное управление) недвижимое имущество, оплатить его цену и выполнить условия в соответствии с настоящим договором.\n" +
                                         $"Недвижимое имущество расположено на земельном участке площадью {accommodation.Area} метров квадратных с номером {accommodation.FlatNumber}.\n" +
                                         "1.2. Продавец передает Покупателю недвижимое имущество свободным от любых прав третьих лиц (с обременением – аренда, залог и т.д.). Недвижимое имущество не состоит под арестом, запрещением.\n" +
                                         "1.3. Оформление документов, удостоверяющих права на земельный участок, необходимый для обслуживания недвижимого имущества, осуществляется в соответствии с законодательством об охране и использовании земель.";
                    body.AppendChild(CreateParagraph(subjectText, JustificationValues.Both));

                    // Цена продажи
                    body.AppendChild(CreateParagraph("2. ЦЕНА ПРОДАЖИ", JustificationValues.Left, true));
                    string priceText = $"2.1. Цена продажи недвижимого имущества, определенная по результатам торгов, составляет {accommodation.Price} белорусских рублей.\n" +
                                       "2.2. Цена продажи недвижимого имущества не включает цену земельного участка (цену права аренды земельного участка), необходимого для его обслуживания.";
                    body.AppendChild(CreateParagraph(priceText, JustificationValues.Both));

                    // Права и обязанности сторон
                    body.AppendChild(CreateParagraph("3. ПРАВА И ОБЯЗАННОСТИ СТОРОН", JustificationValues.Left, true));

                    // Обязанности продавца
                    string sellerObligationsText = "3.1. Продавец обязуется:\n" +
                                                   "3.1.1. В срок, указанный в подпункте 3.1.3 настоящего пункта, составить и представить для подписания Покупателю акт приема-передачи недвижимого имущества;\n" +
                                                   "3.1.2. Предоставить Покупателю документы, которыми располагает Продавец, относящиеся к недвижимому имуществу (технический паспорт, договор аренды, свидетельство (удостоверение) о государственной регистрации и т.п.);\n" +
                                                   "3.1.3. В течение 10 календарных дней после представления Покупателем платежных документов о полной оплате цены продажи недвижимого имущества передать недвижимое имущество Покупателю по акту приема-передачи;\n" +
                                                   "3.1.4. Предоставить Покупателю рассрочку оплаты стоимости недвижимого имущества на срок, указанный в заявлении, но не более трех лет с даты заключения настоящего договора;\n" +
                                                   "3.1.5. Ежемесячно уведомлять Покупателя о применяемых индексах цен и сумме проиндексированного ежемесячного платежа.\n" +
                                                   "3.1.6. В течение 5 рабочих дней со дня представления Покупателем документов, подтверждающих полную оплату стоимости недвижимого имущества, выдать Покупателю справку или иной документ, подтверждающий исполнение обязательства по оплате стоимости недвижимого имущества;\n" +
                                                   "3.1.7. Вносить предложения государственному органу или организации, в подчинении которого он находится, о выполнении обязательного условия, указанного в подпункте 3.2.7 настоящего договора.";
                    body.AppendChild(CreateParagraph(sellerObligationsText, JustificationValues.Both));

                    // Обязанности покупателя
                    string buyerObligationsText = "3.2. Покупатель обязуется:\n" +
                                                  "3.2.1. Обеспечить государственную регистрацию настоящего договора в срок не позднее 10 рабочих дней с даты его подписания;\n" +
                                                  "3.2.2. Произвести оплату цены продажи недвижимого имущества в течение 30 календарных дней с даты заключения настоящего договора;\n" +
                                                  "3.2.3. При приобретении недвижимого имущества для его последующего сноса обратиться за разрешительной документацией на строительство и обеспечить его снос в установленный срок;\n" +
                                                  "3.2.4. Возместить средства Продавца, затраченные на проведение работ по составлению паспорта объекта;\n" +
                                                  "3.2.5. При продаже недвижимого имущества с рассрочкой его оплаты:\n" +
                                                  " - Внести первоначальный платеж в размере, установленном договором, в срок не более 30 календарных дней с даты заключения настоящего договора;\n" +
                                                  " - Оставшуюся сумму оплачивать ежемесячно равными платежами в установленный срок;\n" +
                                                  " - До полной оплаты цены продажи недвижимого имущества оно находится в залоге у Продавца.\n" +
                                                  "3.2.6. Принять недвижимое имущество в собственность в соответствии с актом приема-передачи;\n" +
                                                  "3.2.7. В течение трех лет привести объект в эксплуатацию.\n" +
                                                  "3.2.8. Принимать меры по сохранению недвижимого имущества в соответствии с законодательством Республики Беларусь.";
                    body.AppendChild(CreateParagraph(buyerObligationsText, JustificationValues.Both));

                    // Ответственность сторон
                    body.AppendChild(CreateParagraph("4. ОТВЕТСТВЕННОСТЬ СТОРОН", JustificationValues.Left, true));
                    string responsibilityText = "4.1. В случае нарушения договора Покупателем, Продавец вправе потребовать оплаты и принятия недвижимого имущества или отказаться от исполнения договора.\n" +
                                                "4.2. В случае несвоевременного перечисления средств в оплату недвижимого имущества Покупатель уплачивает пеню за каждый день просрочки.\n" +
                                                "4.3. В случае нарушения обязательств, предусмотренных договором, Покупатель уплачивает пеню за каждый день просрочки.\n" +
                                                "4.4. Неисполнение обязательств является существенным нарушением договора.\n" +
                                                "4.5. При расторжении договора недвижимое имущество возвращается Продавцу, а денежные средства Покупателю за вычетом пени.\n" +
                                                "4.6. Возместить средства Продавца, затраченные на снос недвижимого имущества.";
                    body.AppendChild(CreateParagraph(responsibilityText, JustificationValues.Both));

                    // Заключительные положения
                    body.AppendChild(CreateParagraph("5. ЗАКЛЮЧИТЕЛЬНЫЕ ПОЛОЖЕНИЯ", JustificationValues.Left, true));
                    string finalText = "5.1. Договор вступает в силу с даты его подписания.\n" +
                                       "5.2. Договор может быть изменен или расторгнут по соглашению сторон.\n" +
                                       "5.3. Споры разрешаются в суде по месту нахождения Продавца.\n" +
                                       "5.4. К договору прилагаются:\n" +
                                       "- Паспорт объекта;\n" +
                                       "- Копия договора аренды земельного участка;\n" +
                                       "- Акт приема-передачи недвижимого имущества;\n" +
                                       "- Распорядительный документ на земельный участок.";
                    body.AppendChild(CreateParagraph(finalText, JustificationValues.Both));

                    body.AppendChild(CreateParagraph("\tПодписи Сторон:", "\n", true));
                    string signatures = $"Потребитель:\t\t\t\t\tИсполнитель:\r\n{seller.Surname} {seller.Initials}\t\t\t{currentUser.Surname} {currentUser.Name}\r\n________________________\t\t\t\t\t________________________";
                    body.AppendChild(CreateSingleParagraph(signatures));

                    // Requisites of the parties
                    body.AppendChild(CreateParagraph("8. РЕКВИЗИТЫ И ПОДПИСИ СТОРОН", "\n", true));
                    string requisitesText = $"Потребитель: {seller.Email}, тел. {seller.Phone}\r\n" +
                        $"Паспорт: {seller.PassportNumber}\r\n" +
                        $"(подпись) __________________________ (ФИО) {seller.Surname} {seller.Initials}\r\n" +
                    $"\r\nИсполнитель: {company.Name}, {company.Address}\r\n" +
                    $"р/с {company.AccountNumber} в {company.BankName},\r\n" +
                    $"ИНН {company.INN}, БИК {company.BIK}, Телефон/факс {company.Phone}\r\n" +
                    $"Ответственный по настоящему Договору риэлтер: {currentUser.Surname} {currentUser.Name}\r\n" +
                        $"(подпись) __________________________";
                    body.AppendChild(CreateSingleParagraph(requisitesText));

                    body.AppendChild(CreateParagraph(" ", " ")); // Empty paragraph

                    // Создание и добавление таблицы
                    Table table = new Table();

                    // Установка свойств таблицы (границы)
                    TableProperties tblProps = new TableProperties(
                        new TableBorders(
                            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }));

                    table.AppendChild(tblProps);

                    // Метод для создания строки таблицы
                    TableRow CreateTableRow(string cell1, string cell2)
                    {
                        TableRow tr = new TableRow();
                        TableCell tc1 = new TableCell(new Paragraph(new Run(new Text(cell1))));
                        TableCell tc2 = new TableCell(new Paragraph(new Run(new Text(cell2))));
                        tr.Append(tc1, tc2);
                        return tr;
                    }

                    // Добавление строк в таблицу
                    table.Append(
                        CreateTableRow("Сведения об объекте договора", ""),
                        CreateTableRow("Место нахождения недвижимого имущества:", accommodation.Adress),
                        CreateTableRow("Адрес:", accommodation.Adress),
                        CreateTableRow("Площадь:", accommodation.Area.ToString()),
                        CreateTableRow("Количество помещений:", accommodation.AmountOfRooms.ToString()),
                        CreateTableRow("Срок действия договора", ""),
                        CreateTableRow("Начало:", startDate.ToString("dd.MM.yyyy")),
                        CreateTableRow("Окончание:", endDate.ToString("dd.MM.yyyy")),
                        CreateTableRow("Сведения о риелторе:", ""),
                        CreateTableRow("ФИО", $"{currentUser.Surname} {currentUser.Name}"),
                        CreateTableRow("Число, месяц, год рождения", ""),
                        CreateTableRow("Идентификационный номер", $"{currentUser.IdentificationNumber}"),
                        CreateTableRow("Моб. номер", ""),
                        CreateTableRow("Почта", $"{currentUser.Email}"),
                        CreateTableRow("Сведения о продавце:", ""),
                        CreateTableRow("ФИО", $"{seller.Surname} {seller.Initials}"),
                        CreateTableRow("Число, месяц, год рождения", ""),
                        CreateTableRow("Идентификационный номер", seller.IdentificationNumber),
                        CreateTableRow("Моб. номер", $"{seller.Phone}"),
                        CreateTableRow("Почта", $"{seller.Email}"),
                        CreateTableRow("Подпись", ""),
                        CreateTableRow("Должность", ""));

                    body.AppendChild(table);

                    mainPart.Document.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        private static Paragraph CreateParagraph(string text, JustificationValues justification, bool bold = false)
        {
            Paragraph paragraph = new Paragraph();
            Run run = new Run();

            RunProperties runProperties = new RunProperties();
            if (bold)
            {
                runProperties.Append(new Bold());
            }
            run.Append(runProperties);
            run.Append(new Text(text));

            paragraph.Append(run);

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Justification = new Justification() { Val = justification };
            paragraph.Append(paragraphProperties);

            return paragraph;
        }
        private Paragraph CreateParagraph(string text, string after, bool bold = false)
        {
            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            if (bold)
            {
                RunProperties runProperties = new RunProperties();
                Bold boldElement = new Bold();
                runProperties.Append(boldElement);
                run.Append(runProperties);
            }
            run.Append(new Text(text));
            paragraph.Append(run);
            return paragraph;
        }

        private Paragraph CreateSingleParagraph(string text)
        {
            Paragraph paragraph = new Paragraph(new Run(new Text(text)));
            return paragraph;
        }

    }
}
