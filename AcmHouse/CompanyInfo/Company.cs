using AcmHouse.AccommodationExtraInfo;
using AcmHouse.AccommodationInfo;
using AcmHouse.UsersInfo;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;

namespace AcmHouse.CompanyInfo
{
    public class Company
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string CorrespondentAccount { get; set; }
        public string BIK { get; set; }
        public string INN { get; set; }

        public string OwnerInitials { get; set; }

        public Company()
        {
            Name = "ПроКвартал";
            Address = "Беларусь, Минск, ул.Карла Либкнехта 129, 220036";
            Phone = "+375 44 547 22 52 ";
            Email = "prokvartal@mail.ru";
            BankName = "Альфа-Банк";
            AccountNumber = "BY51ALPHA301840GHB70010290000";
            CorrespondentAccount = "BY51ALPHA2873197VTB40020130000";
            BIK = "153001550";
            INN = "790110246";
            OwnerInitials = "Романовский Александр Дмитриевич";
        }

        public void GenerateRentalDocument(string outputPath, Accommodation accommodation, AccommodationDetails accommodationDetails, CurrentUser currentUser, string surname, string initials, string email, string phone, string passportNumber, string identificationNumber, DateTime startDate, DateTime endDate, int days, double total)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();

                Body body = mainPart.Document.AppendChild(new Body());

                string flatOwnerName = "Иван";
                string flatOwnerSurname = "Войтович";
                string flatOwnerPatronymic = "Игнатьев";
                Paragraph titleParagraph = CreateParagraph("\t\t\t\tДОГОВОР\r\nнайма жилого помещения частного жилищного фонда граждан", "");
                body.AppendChild(titleParagraph);

                string contractText = $"Собственник жилого помещения, {flatOwnerName} {flatOwnerSurname} {flatOwnerPatronymic}, именуемый в дальнейшем наймодатель, с одной стороны, и гражданин(ка) {surname} {initials}, именуемый(ая) в дальнейшем наниматель, с другой стороны, заключили настоящий \r\nдоговор о следующем:\r\n";
                body.AppendChild(CreateParagraph(contractText, ""));

                body.AppendChild(CreateSingleParagraph($"1. Наймодатель предоставляет нанимателю во владение и пользование за плату жилое помещение для проживания в нем на срок с {startDate} по {endDate} адресу {accommodationDetails.Address} , квартира N {accommodationDetails.FlatNumber}, площадью {accommodation.Area} с количеством комнат {accommodation.AmountOfRooms}"));
                body.AppendChild(CreateSingleParagraph("\nЖилое помещение отапливается, имеет естественное освещение и соответствует обязательным для соблюдения санитарным нормам и правилам, гигиеническим нормативам и иным обязательным для соблюдения техническим нормативным правовым актам, содержащим требования, предъявляемые к жилым помещениям. Инженерное оборудование находится в исправном состоянии.\r\nПраво собственности наймодателя на жилое помещение зарегистрировано в установленном порядке в едином государственном регистре недвижимого имущества, прав на него и сделок с ним. Копия технического паспорта (выкопировка из технического паспорта) на жилое помещение прилагается к настоящему договору и является его неотъемлемой частью.\n"));
                body.AppendChild(CreateSingleParagraph($"В указанном жилом помещении постоянно проживают:\r\n {flatOwnerSurname} {flatOwnerName} {flatOwnerPatronymic}"));
                body.AppendChild(CreateSingleParagraph($"Наймодатель предоставляет во владение и пользование нанимателю сове имущество"));

                // Добавление информации о предмете договора
                string rightEmployer = "\n\t\t\t\tПрава и обязанности нанимателя";
                body.AppendChild(CreateParagraph(rightEmployer, "\n"));
                body.AppendChild(CreateSingleParagraph($"2.1. предоставлять право владения и пользования занимаемым им жилым помещением членам своей семьи и иным гражданам в соответствии с законодательством;"));
                body.AppendChild(CreateSingleParagraph($"2.2. в любое время с согласия проживающих совместно с ним совершеннолетних членов его семьи расторгнуть настоящий договор, выполнив свои обязательства перед наймодателем;"));
                body.AppendChild(CreateSingleParagraph($"2.3. осуществлять иные права, предусмотренные законодательством."));

                body.AppendChild(CreateSingleParagraph($"3. Наниматель обязан:"));
                body.AppendChild(CreateSingleParagraph($"3.1. использовать жилое, подсобные и вспомогательные помещения, а также находящееся в них оборудование в соответствии с их назначением;"));
                body.AppendChild(CreateSingleParagraph($"3.2. вносить плату за пользование жилым помещением в сроки  \r\n\r\nв размере  "));
                body.AppendChild(CreateSingleParagraph($"3.3. вносить плату за жилищно-коммунальные услуги за каждый истекший месяц не позднее 25-го числа следующего за ним месяца, если иное не установлено настоящим договором или законодательными актами, на основании платежных документов, предоставляемых в соответствии с законодательством;"));

                // Добавление прав и обязанностей сторон
                string rightsLanlord = "\n\t\t\t\tПрава и обязанности наймодателя";
                body.AppendChild(CreateParagraph(rightsLanlord, "\n"));
                body.AppendChild(CreateSingleParagraph($"4. Наймодатель имеет право требовать:"));
                body.AppendChild(CreateSingleParagraph($"4.1. своевременного и в полном объеме внесения нанимателем платы за пользование жилым помещением и за жилищно-коммунальные услуги, возмещения расходов организаций, осуществляющих эксплуатацию жилищного фонда и (или) предоставляющих жилищно-коммунальные услуги, на электроэнергию, потребляемую на освещение вспомогательных помещений и работу оборудования, в том числе лифтов, в многоквартирных жилых домах, если такие услуги и возмещение расходов оплачиваются нанимателем, платы за предоставленное во владение и пользование имущество и оказываемые услуги, если такие услуги оказываются;\r\n"));
                body.AppendChild(CreateSingleParagraph($"4.2. использования нанимателем предоставленного ему во владение и пользование жилого помещения в соответствии с его назначением;"));
                body.AppendChild(CreateSingleParagraph($"4.3. предоставления доступа в жилое помещение (в согласованное с нанимателем время и в его присутствии) работников организаций, осуществляющих эксплуатацию жилищного фонда и (или) предоставляющих жилищно-коммунальные услуги, для проверки технического состояния помещения, инженерных систем и оборудования, проведения ремонтных работ;\r\n"));
                body.AppendChild(CreateSingleParagraph($"5. Наймодатель обязан:"));
                body.AppendChild(CreateSingleParagraph($"5.1. зарегистрировать настоящий договор до установленного в нем срока фактического предоставления в наем жилого помещения в районном, городском, поселковом, сельском исполкомах, местной администрации района в городе;"));
                body.AppendChild(CreateSingleParagraph($"5.2. предоставить нанимателю по акту о приеме-передаче во владение и пользование либо без такого акта жилое помещение;"));
                body.AppendChild(CreateSingleParagraph($"5.3. производить по мере необходимости в согласованное с нанимателем время:\r\nосмотр жилого помещения, санитарно-технического и иного оборудования, находящегося в нем;\r\nза свой счет текущий ремонт жилого помещения;\r\n"));

                string responsibilitiesOfTheParties = "\n\t\t\t\tОтветственность сторон\r\n ";
                body.AppendChild(CreateParagraph(responsibilitiesOfTheParties, "\n"));
                body.AppendChild(CreateSingleParagraph("6. В случае неисполнения или ненадлежащего исполнения обязательств, предусмотренных настоящим договором, виновная сторона возмещает в соответствии с законодательством другой стороне понесенные убытки."));

                string settlementProcedure = "\n\t\t\t\tВступление в силу договора ";
                body.AppendChild(CreateParagraph(settlementProcedure, "\n"));
                body.AppendChild(CreateSingleParagraph("7. Настоящий договор вступает в силу с даты его регистрации в районном, городском, поселковом, сельском исполкомах, местной администрации района в городе.\r\n"));

                string contractTime = "\n\t\t\t\tСрок действия договора";
                body.AppendChild(CreateParagraph(contractTime, "\n"));
                body.AppendChild(CreateSingleParagraph($"6.1.Настоящий договор вступает в силу с {startDate} и действует по {endDate}"));
                body.AppendChild(CreateSingleParagraph("6.2.Наймодатель вправе расторгнуть договор в любое время, письменно предупредив Нанимателя за 3(три) дня до предполагаемой даты расторжения договора, в таком случае Наниматель возвращает жилье в порядке, а Наймодатель обеспечивает перерасчет арендной платы при досрочном расторжении настоящего договора."));

                string details = "\n7. АДРЕСА И РЕКВИЗИТЫ СТОРОН";
                body.AppendChild(CreateParagraph(details, "\n"));

                body.AppendChild(CreateParagraph("\nНаймодатель", "\n"));

                body.AppendChild(CreateParagraph("Company: ", Name));
                body.AppendChild(CreateParagraph("Адрес: ", Address));
                body.AppendChild(CreateParagraph("Номер телефона: ", Phone));
                body.AppendChild(CreateParagraph("Электронный адрес: ", Email));
                body.AppendChild(CreateParagraph("Банк: ", BankName));
                body.AppendChild(CreateParagraph("Расчетный счет организации: ", AccountNumber));
                body.AppendChild(CreateParagraph("Коррекспондентский счет организации: ", CorrespondentAccount));
                body.AppendChild(CreateParagraph("БИК: ", BIK));
                body.AppendChild(CreateParagraph("ИНН: ", INN));

                body.AppendChild(CreateParagraph("\nВладелец жилья", "\n"));
                body.AppendChild(CreateParagraph("Имя: ", flatOwnerName));
                body.AppendChild(CreateParagraph("Фамилия: ", flatOwnerSurname));
                body.AppendChild(CreateParagraph("Отчество: ", flatOwnerPatronymic));

                body.AppendChild(CreateParagraph(" ", " ")); // Пустой абзац

                // Добавление информации о клиенте
                body.AppendChild(CreateParagraph("\nАРЕНДАТОР", "\n"));

                body.AppendChild(CreateParagraph("Фамилия: ", surname));
                body.AppendChild(CreateParagraph("Инициалы: ", initials));
                body.AppendChild(CreateParagraph("Электронный адрес: ", email));
                body.AppendChild(CreateParagraph("Номер паспорта: ", passportNumber));
                body.AppendChild(CreateParagraph("Идентификационный номер: ", identificationNumber));

                body.AppendChild(CreateParagraph(" ", " ")); // Пустой абзац

                // Таблица с данными об объекте и участниках
                Table table = new Table();

                // Установка границ таблицы
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
                    CreateTableRow("Место нахождения недвижимого имущества:", accommodationDetails.Address),
                    CreateTableRow("Адрес:", accommodationDetails.Address),
                    CreateTableRow("Площадь:", accommodation.Area.ToString()),
                    CreateTableRow("Количество комнат:", accommodation.AmountOfRooms.ToString()),
                    CreateTableRow("Срок действия договора", ""),
                    CreateTableRow("Начало:", startDate.ToString("dd.MM.yyyy")),
                    CreateTableRow("Окончание:", endDate.ToString("dd.MM.yyyy")),
                    CreateTableRow("Сведения о риелторе:", ""),
                    CreateTableRow("ФИО", $"{currentUser.Surname} {currentUser.Name}"),
                    CreateTableRow("Число, месяц, год рождения", ""),
                    CreateTableRow("Идентификационный номер", $"{currentUser.IdentificationNumber}"),
                    CreateTableRow("Моб. номер", ""),
                    CreateTableRow("Почта", $"{currentUser.Email}"),
                    CreateTableRow("Сведения об арендаторе:", ""),
                    CreateTableRow("ФИО", $"{surname} {initials}"),
                    CreateTableRow("Число, месяц, год рождения", ""),
                    CreateTableRow("Идентификационный номер", identificationNumber),
                    CreateTableRow("Моб. номер", $"{phone}"),
                    CreateTableRow("Почта", $"{email}"),
                    CreateTableRow("Подпись", ""),
                    CreateTableRow("Должность", ""));

                body.Append(table);

                // Сохранение изменений и закрытие документа
                document.Save();
            }
        }

        private Paragraph CreateParagraph(string label, string value)
        {
            // Создание абзаца с меткой и значением
            Paragraph paragraph = new Paragraph();

            // Создание стиля для метки
            RunProperties labelRunProperties = new RunProperties();
            Bold bold = new Bold();
            labelRunProperties.Append(bold);

            // Создание элемента Run для метки
            Run labelRun = new Run(new Text(label));
            labelRun.RunProperties = labelRunProperties;
            paragraph.AppendChild(labelRun);

            // Создание элемента Run для значения
            Run valueRun = new Run(new Text(value));
            paragraph.AppendChild(valueRun);

            return paragraph;
        }
        private Paragraph CreateSingleParagraph(string label)
        {
            // Создание абзаца с меткой и значением
            Paragraph paragraph = new Paragraph();

            // Создание стиля для метки
            RunProperties labelRunProperties = new RunProperties();
            Bold bold = new Bold();
            labelRunProperties.Append(bold);

            // Создание элемента Run для метки
            Run labelRun = new Run(new Text(label));
            labelRun.RunProperties = labelRunProperties;
            paragraph.AppendChild(labelRun);
            return paragraph;
        }
    }
}
