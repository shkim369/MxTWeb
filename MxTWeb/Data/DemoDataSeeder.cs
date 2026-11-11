using Microsoft.EntityFrameworkCore;
using MxTWeb.Enums;
using MxTWeb.Models;

namespace MxTWeb.Data
{
    /// <summary>
    /// 로컬 개발/데모용 더미 데이터 시더.
    /// - Development 환경 + 구성값 DemoData:Seed = true 일 때만 Program.cs 에서 호출된다.
    /// - 테이블(또는 테이블 묶음)이 비어 있을 때만 채우므로 여러 번 실행해도 중복되지 않는다.
    /// - 날짜는 오늘 기준 상대값이라 대시보드의 "최근 7일" 통계에도 값이 표시된다.
    /// </summary>
    public static class DemoDataSeeder
    {
        // 고정 시드: 실행할 때마다 같은 데이터가 생성되도록
        private static readonly Random Rng = new(20261111);
        private static DateTime Today => DateTime.Today;

        private static readonly string[] Staff = { "김민준", "이서연", "박지호", "최수아", "정예준", "강하은", "윤도윤", "임지유" };
        private static readonly string[] Departments = { "연구1팀", "연구2팀", "생산팀", "품질관리팀", "개발팀" };

        public static async Task SeedAsync(IServiceProvider services, ILogger logger)
        {
            var webFactory = services.GetRequiredService<IDbContextFactory<MxTWebContext>>();
            var loginFactory = services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

            await using (var db = await webFactory.CreateDbContextAsync())
            {
                await SeedCyropreservedAsync(db, logger);
                await SeedReagentsAsync(db, logger);
                await SeedEquipmentAsync(db, logger);
                await SeedComponentsAsync(db, logger);
                await SeedShipmentsAsync(db, logger);
                await SeedChipsAsync(db, logger);
            }

            await using (var loginDb = await loginFactory.CreateDbContextAsync())
            {
                await SeedUpdatePostsAsync(loginDb, logger);
            }
        }

        private static T Pick<T>(IReadOnlyList<T> items) => items[Rng.Next(items.Count)];
        private static DateTime DaysAgo(int min, int max) => Today.AddDays(-Rng.Next(min, max + 1));

        // ───────────────────────── 세포 동결 보관 ─────────────────────────
        private static async Task SeedCyropreservedAsync(MxTWebContext db, ILogger logger)
        {
            if (await db.Cyropreserved.AnyAsync()) return;

            var primary = new (string Name, string Origin)[]
            {
                ("Human PBMC", "Human peripheral blood"), ("Human T cell (CD3+)", "Human peripheral blood"),
                ("Human NK cell", "Human peripheral blood"), ("Human Monocyte (CD14+)", "Human peripheral blood"),
                ("Human HUVEC", "Human umbilical vein"), ("Human Dermal Fibroblast", "Human skin"),
                ("Human Hepatocyte", "Human liver"), ("Human MSC (BM)", "Human bone marrow"),
                ("Mouse Splenocyte", "C57BL/6 spleen"), ("Mouse BMDC", "C57BL/6 bone marrow"),
                ("Rat Cortical Neuron", "SD rat embryo cortex"), ("Human Keratinocyte", "Human foreskin"),
            };
            var lines = new (string Name, string Origin, string Supplier)[]
            {
                ("HeLa", "Human cervix adenocarcinoma", "ATCC"), ("HEK293T", "Human embryonic kidney", "ATCC"),
                ("Jurkat E6.1", "Human T lymphocyte leukemia", "ATCC"), ("K562", "Human CML", "KCLB"),
                ("A549", "Human lung carcinoma", "KCLB"), ("MCF-7", "Human breast adenocarcinoma", "ATCC"),
                ("HepG2", "Human hepatocellular carcinoma", "KCLB"), ("CHO-K1", "Chinese hamster ovary", "ATCC"),
                ("THP-1", "Human monocytic leukemia", "KCLB"), ("Raji", "Human Burkitt's lymphoma", "KCLB"),
                ("NIH/3T3", "Mouse embryonic fibroblast", "ATCC"), ("U-87 MG", "Human glioblastoma", "ATCC"),
                ("Vero", "African green monkey kidney", "KCLB"), ("SH-SY5Y", "Human neuroblastoma", "ATCC"),
            };
            string[] primarySuppliers = { "Lonza", "STEMCELL Technologies", "PromoCell", "Gibco", "자체 분리" };
            string[] tests = { "Mycoplasma 음성", "Viability 95%", "Viability 92%", "Mycoplasma 음성 / STR 확인", "미실시" };

            var list = new List<Cyropreserved>();
            int seq = 1;

            foreach (var p in primary)
            {
                for (int i = 0; i < Rng.Next(1, 3); i++)
                {
                    var receipt = DaysAgo(5, 540);
                    list.Add(new Cyropreserved
                    {
                        Code = $"PC-{seq:D3}",
                        Division = "Primary cell",
                        Name = p.Name,
                        ReceiptDate = receipt,
                        Recipient = Pick(Staff),
                        Supplier = Pick(primarySuppliers),
                        FreezeDate = receipt.AddDays(Rng.Next(0, 4)),
                        Quantity = Rng.Next(0, 12),
                        Lot = $"{Rng.Next(100000, 999999)}",
                        Density = $"{Rng.Next(1, 11)} x 10^6 cells/vial",
                        Location1 = $"LN2 Tank {Rng.Next(1, 3)}",
                        Location2 = $"Rack {(char)('A' + Rng.Next(0, 5))}-Box {Rng.Next(1, 9)}",
                        Test = Pick(tests),
                        Origin = p.Origin,
                    });
                    seq++;
                }
            }

            foreach (var c in lines)
            {
                int gen = Rng.Next(3, 8);
                for (int i = 0; i < Rng.Next(1, 3); i++)
                {
                    var receipt = DaysAgo(3, 720);
                    list.Add(new Cyropreserved
                    {
                        Code = $"CL-{seq:D3}",
                        Division = "Cell line",
                        Name = c.Name,
                        ReceiptDate = receipt,
                        Recipient = Pick(Staff),
                        Supplier = c.Supplier,
                        FreezeDate = receipt.AddDays(Rng.Next(3, 15)),
                        Quantity = Rng.Next(0, 15),
                        Lot = $"{c.Supplier}-{Rng.Next(10000, 99999)}",
                        Density = $"{Rng.Next(1, 6)} x 10^6 cells/vial",
                        Location1 = "Deep Freezer (-150℃)",
                        Location2 = $"Shelf {Rng.Next(1, 5)}-Box {Rng.Next(1, 13)}",
                        Test = Pick(tests),
                        Parent = i == 0 ? "Original" : $"{c.Name} P{gen - 1}",
                        Generation = $"P{gen + i * 2}",
                        Origin = c.Origin,
                        Note = Rng.Next(0, 4) == 0 ? "해동 시 37℃ water bath 2분 이내" : null,
                    });
                    seq++;
                }
            }

            db.Cyropreserved.AddRange(list);
            await db.SaveChangesAsync();

            // 승인 워크플로 요청: 대기/승인/반려 섞어서
            string[] purposes = { "세포 독성 실험", "칩 배양 테스트", "Flow cytometry 분석", "공동연구기관 분양", "Co-culture 실험", "약물 스크리닝" };
            var stocked = list.Where(x => x.Quantity > 0).ToList();
            var requests = new List<CyropreservedRequest>();
            for (int i = 0; i < 14; i++)
            {
                var target = Pick(stocked);
                var status = i < 5 ? RequestStatusEnum.Pending : (i < 11 ? RequestStatusEnum.Approved : RequestStatusEnum.Rejected);
                var applied = DaysAgo(0, 45);
                requests.Add(new CyropreservedRequest
                {
                    CyropreservedId = target.Id,
                    Applicant = Pick(Staff),
                    ApplicationDate = applied,
                    RequestedQuantity = Rng.Next(1, Math.Min(4, target.Quantity) + 1),
                    Purpose = Pick(purposes),
                    Status = status,
                    Approver = status == RequestStatusEnum.Pending ? null : "admin@mxtbiotech.com",
                    ProcessedDate = status == RequestStatusEnum.Pending ? null : applied.AddDays(Rng.Next(0, 3)).AddHours(Rng.Next(9, 18)),
                    RejectionReason = status == RequestStatusEnum.Rejected ? "재고 부족 - 차주 추가 입고 후 재신청 바랍니다." : null,
                    CreatedDate = applied.AddHours(Rng.Next(9, 18)),
                });
            }
            db.CyropreservedRequest.AddRange(requests);
            await db.SaveChangesAsync();

            logger.LogInformation("[DemoData] Cyropreserved {Count}건, 요청 {ReqCount}건 생성", list.Count, requests.Count);
        }

        // ───────────────────────── 시약 ─────────────────────────
        private static async Task SeedReagentsAsync(MxTWebContext db, ILogger logger)
        {
            if (await db.Reagent.AnyAsync()) return;

            var items = new (string Name, string Maker, string Amount, string Cat)[]
            {
                ("DMEM, high glucose", "Gibco", "500 mL", "11965-092"),
                ("RPMI 1640 Medium", "Gibco", "500 mL", "11875-093"),
                ("Fetal Bovine Serum", "Gibco", "500 mL", "16000-044"),
                ("Penicillin-Streptomycin (10,000 U/mL)", "Gibco", "100 mL", "15140-122"),
                ("Trypsin-EDTA (0.25%)", "Gibco", "100 mL", "25200-056"),
                ("DPBS, no calcium, no magnesium", "Gibco", "500 mL", "14190-144"),
                ("DMSO, Hybri-Max", "Sigma-Aldrich", "50 mL", "D2650"),
                ("Trypan Blue Solution 0.4%", "Sigma-Aldrich", "100 mL", "T8154"),
                ("Bovine Serum Albumin", "Sigma-Aldrich", "50 g", "A7906"),
                ("Paraformaldehyde 4% in PBS", "Biosesang", "500 mL", "PC2031-100-00"),
                ("Triton X-100", "Sigma-Aldrich", "100 mL", "T8787"),
                ("DAPI", "Thermo Fisher", "10 mg", "D1306"),
                ("Calcein AM", "Thermo Fisher", "1 mg", "C3099"),
                ("Ethidium Homodimer-1", "Thermo Fisher", "1 mg", "E1169"),
                ("CellTiter-Glo 2.0", "Promega", "100 mL", "G9242"),
                ("Matrigel Matrix, GFR", "Corning", "10 mL", "354230"),
                ("Collagen I, Rat Tail", "Corning", "100 mg", "354236"),
                ("Fibronectin, Human", "Corning", "1 mg", "356008"),
                ("PDMS Sylgard 184 Kit", "Dow", "1.1 kg", "4019862"),
                ("Pluronic F-127", "Sigma-Aldrich", "250 g", "P2443"),
                ("TRIzol Reagent", "Invitrogen", "100 mL", "15596026"),
                ("PowerUp SYBR Green Master Mix", "Applied Biosystems", "5 mL", "A25742"),
                ("RNeasy Mini Kit", "Qiagen", "50 preps", "74104"),
                ("Human IL-6 ELISA Kit", "R&D Systems", "96 wells", "D6050"),
                ("Human TNF-alpha ELISA Kit", "R&D Systems", "96 wells", "DTA00D"),
                ("Recombinant Human IL-2", "PeproTech", "50 ug", "200-02"),
                ("Anti-CD3 (OKT3)", "BioLegend", "100 ug", "317326"),
                ("Ficoll-Paque PLUS", "Cytiva", "500 mL", "17144002"),
                ("Ethanol 99.9%", "덕산", "4 L", "4023"),
                ("Isopropanol", "덕산", "4 L", "5035"),
            };
            string[] locations = { "냉장고 1 (4℃)", "냉장고 2 (4℃)", "냉동고 1 (-20℃)", "Deep Freezer (-80℃)", "시약장 A", "시약장 B", "인화성 시약장" };

            var list = items.Select((x, i) => new Reagent
            {
                Code = $"R-{i + 1:D3}",
                Name = x.Name,
                Manufacturer = x.Maker,
                Amount = x.Amount,
                Cat = x.Cat,
                ReceiptDate = DaysAgo(1, 300),
                Quantity = Rng.Next(0, 9),
                Lot = $"{(char)('A' + Rng.Next(0, 26))}{Rng.Next(1000000, 9999999)}",
                Location = Pick(locations),
                Comment = Rng.Next(0, 5) == 0 ? "개봉 후 1개월 내 사용" : null,
            }).ToList();

            db.Reagent.AddRange(list);
            await db.SaveChangesAsync();
            logger.LogInformation("[DemoData] Reagent {Count}건 생성", list.Count);
        }

        // ───────────────────────── 장비 ─────────────────────────
        private static async Task SeedEquipmentAsync(MxTWebContext db, ILogger logger)
        {
            if (await db.Equipment.AnyAsync()) return;

            var items = new (string Name, string Model, string Maker, string Seller, string Location, string Mgmt)[]
            {
                ("CO2 Incubator", "Heracell VIOS 160i", "Thermo Fisher", "코아바이오", "세포배양실", "월 1회 CO2 농도 점검"),
                ("CO2 Incubator", "MCO-170AICUV", "PHCbi", "코아바이오", "세포배양실", "월 1회 CO2 농도 점검"),
                ("Biosafety Cabinet", "1300 Series A2", "Thermo Fisher", "랩코리아", "세포배양실", "연 1회 HEPA 필터 인증"),
                ("Biosafety Cabinet", "CHC-777A2", "CHC Lab", "CHC Lab", "세포배양실", "연 1회 HEPA 필터 인증"),
                ("Inverted Microscope", "Eclipse Ts2", "Nikon", "니콘인스트루먼트코리아", "세포배양실", "분기별 광학계 청소"),
                ("Fluorescence Microscope", "ECLIPSE Ti2-E", "Nikon", "니콘인스트루먼트코리아", "이미징실", "광원 사용시간 기록"),
                ("Confocal Microscope", "LSM 900", "Zeiss", "칼자이스", "이미징실", "예약제 운영"),
                ("Centrifuge", "Sorvall ST 16R", "Thermo Fisher", "랩코리아", "실험실 A", "로터 사용횟수 관리"),
                ("Micro Centrifuge", "5425 R", "Eppendorf", "에펜도르프코리아", "실험실 A", "-"),
                ("Real-Time PCR", "QuantStudio 5", "Applied Biosystems", "써모피셔코리아", "분자생물실", "연 1회 Calibration"),
                ("Flow Cytometer", "CytoFLEX S", "Beckman Coulter", "벡크만쿨터코리아", "분석실", "매일 QC bead 측정"),
                ("Microplate Reader", "SpectraMax iD5", "Molecular Devices", "몰레큘라디바이스코리아", "분석실", "-"),
                ("Cell Counter", "Countess 3", "Invitrogen", "써모피셔코리아", "세포배양실", "-"),
                ("Plasma Cleaner", "CUTE-1MPR", "Femto Science", "펨토사이언스", "클린룸", "챔버 주 1회 청소"),
                ("Spin Coater", "ACE-200", "DONG AH Trade", "동아무역", "클린룸", "-"),
                ("Mask Aligner", "MDA-400S", "MIDAS System", "마이다스시스템", "클린룸", "UV 램프 사용시간 관리"),
                ("Syringe Pump", "Fusion 200", "Chemyx", "케미엑스코리아", "칩 테스트실", "-"),
                ("Pressure Controller", "OB1 MK4", "Elveflow", "Elveflow", "칩 테스트실", "압력 센서 분기별 점검"),
                ("Autoclave", "HK-AC60", "한백과학", "한백과학", "준비실", "주 1회 Bowie-Dick 테스트"),
                ("Deep Freezer (-80℃)", "TSX60086A", "Thermo Fisher", "랩코리아", "준비실", "온도 로거 상시 모니터링"),
                ("LN2 Tank", "LS750", "Taylor-Wharton", "대성산업가스", "준비실", "주 1회 LN2 보충"),
                ("Ultrapure Water System", "Milli-Q IQ 7000", "Merck", "머크", "준비실", "필터 6개월 교체"),
            };

            var list = items.Select((x, i) => new Equipment
            {
                ManageCode = $"EQ-{2019 + i % 7}-{i + 1:D3}",
                Name = x.Name,
                Model = x.Model,
                Manufacturer = x.Maker,
                PurchaseDate = DaysAgo(20, 2200),
                Seller = x.Seller,
                Location = x.Location,
                Management = x.Mgmt,
                Manager_Main = Pick(Staff),
                Manager_Sub = Pick(Staff),
                Note = Rng.Next(0, 6) == 0 ? "유지보수 계약 진행중" : null,
            }).ToList();

            db.Equipment.AddRange(list);
            await db.SaveChangesAsync();
            logger.LogInformation("[DemoData] Equipment {Count}건 생성", list.Count);
        }

        // ───────────────────────── 부품 / 입출고 이력 / BOM ─────────────────────────
        private static async Task SeedComponentsAsync(MxTWebContext db, ILogger logger)
        {
            if (await db.Component.AnyAsync()) return;

            // (Product, Assembly, Description, Model, Category, Maker, Material, Price, Currency, BomQty)
            var parts = new (string Product, string Assembly, string Desc, string Model, string Cat, string Maker, string Mat, decimal Price, string Cur, int BomQty)[]
            {
                ("Ordyh", "Main Frame", "Base Plate", "BP-300", "기구", "자체 설계", "AL6061", 85000m, "KRW", 1),
                ("Ordyh", "Main Frame", "Side Cover", "SC-300L", "기구", "자체 설계", "ABS", 32000m, "KRW", 2),
                ("Ordyh", "Main Frame", "Top Cover", "TC-300", "기구", "자체 설계", "PC", 41000m, "KRW", 1),
                ("Ordyh", "Main Frame", "Hex Bolt M4x10", "M4-10", "체결", "미스미", "SUS304", 120m, "KRW", 24),
                ("Ordyh", "Fluidic", "Solenoid Valve 2-Way", "LHDA0533115H", "유체", "The Lee Company", "PEEK", 98.5m, "USD", 4),
                ("Ordyh", "Fluidic", "Micro Diaphragm Pump", "NF 5", "유체", "KNF", "PPS", 215m, "USD", 1),
                ("Ordyh", "Fluidic", "Pressure Sensor", "ABPDANV015PGAA5", "센서", "Honeywell", "-", 38.2m, "USD", 2),
                ("Ordyh", "Fluidic", "PTFE Tubing 1/16\"", "TUB-1/16-PTFE", "유체", "IDEX", "PTFE", 4500m, "KRW", 3),
                ("Ordyh", "Fluidic", "Luer Lock Fitting", "P-624", "유체", "IDEX", "PEEK", 12.4m, "USD", 8),
                ("Ordyh", "Control Board", "MCU Board", "MXT-MCU-R3", "전장", "자체 설계", "FR4", 120000m, "KRW", 1),
                ("Ordyh", "Control Board", "Stepper Driver", "TMC2209", "전장", "Trinamic", "-", 6.8m, "USD", 2),
                ("Ordyh", "Control Board", "Power Supply 24V", "LRS-100-24", "전장", "MEAN WELL", "-", 28000m, "KRW", 1),
                ("Ordyh", "Control Board", "Touch LCD 7\"", "HMI-070", "전장", "Waveshare", "-", 64.0m, "USD", 1),
                ("Ordyh", "Thermal", "Peltier Module", "TEC1-12706", "열관리", "Hebei", "-", 4200m, "KRW", 2),
                ("Ordyh", "Thermal", "Heat Sink", "HS-60x60", "열관리", "자체 설계", "AL6063", 9800m, "KRW", 2),
                ("Ordyh", "Thermal", "Cooling Fan 60mm", "AFB0612HH", "열관리", "Delta", "-", 7600m, "KRW", 2),
                ("Ordyh", "Optics", "LED Module 488nm", "LED-488-1W", "광학", "Thorlabs", "-", 182m, "USD", 1),
                ("Ordyh", "Optics", "CMOS Camera", "acA1920-40um", "광학", "Basler", "-", 520m, "USD", 1),
                ("Ordyh", "Optics", "Objective Lens 10x", "PLN10X", "광학", "Olympus", "-", 690000m, "KRW", 1),
                ("RUO", "Chip Holder", "Chip Holder Base", "CH-B01", "기구", "자체 설계", "AL6061", 45000m, "KRW", 1),
                ("RUO", "Chip Holder", "Chip Clamp", "CH-C01", "기구", "자체 설계", "POM", 18000m, "KRW", 2),
                ("RUO", "Chip Holder", "O-Ring 3x1", "AS568-003", "체결", "NOK", "FKM", 350m, "KRW", 8),
                ("RUO", "Chip Holder", "Thumb Screw M3", "TS-M3", "체결", "미스미", "SUS304", 900m, "KRW", 4),
                ("RUO", "Reservoir", "Media Reservoir 15mL", "RS-15", "소모품", "자체 설계", "PP", 2500m, "KRW", 2),
                ("RUO", "Reservoir", "Reservoir Cap", "RS-CAP", "소모품", "자체 설계", "PP", 600m, "KRW", 2),
                ("RUO", "Reservoir", "Sterile Filter 0.22um", "SLGP033RS", "소모품", "Merck", "PES", 3800m, "KRW", 2),
                ("RUO", "Packaging", "Sterile Pouch", "SP-200", "포장", "한국멸균", "PET/PE", 450m, "KRW", 1),
                ("RUO", "Packaging", "Product Box", "PB-RUO-01", "포장", "대한포장", "골판지", 1800m, "KRW", 1),
                ("RUO", "Packaging", "User Manual", "UM-RUO-v2", "포장", "자체 제작", "인쇄물", 1500m, "KRW", 1),
            };
            string[] sellers = { "디바이스마트", "미스미코리아", "Digi-Key", "Mouser", "엘레파츠", "직거래" };
            string[] leadTimes = { "3일", "1주", "2주", "3~4주", "6주" };

            var components = parts.Select((p, i) =>
            {
                int safety = p.BomQty * Rng.Next(3, 8);
                // 일부 부품은 재고 부족 상태로 만들어 대시보드 "부족 부품" 카드에 표시되게 함
                int stock = i % 6 == 0 ? Rng.Next(0, safety) : safety + Rng.Next(0, safety * 2);
                return new Component
                {
                    Product = p.Product,
                    Assembly = p.Assembly,
                    PartNumber = $"{(p.Product == "Ordyh" ? "ORD" : "RUO")}-{p.Assembly[..2].ToUpper()}-{i + 1:D3}",
                    Description = p.Desc,
                    Model = p.Model,
                    Origin = p.Cur == "USD" ? "USA" : "KOREA",
                    Quantity = safety,
                    Price = p.Price,
                    Currency = p.Cur,
                    Category = p.Cat,
                    Maker = p.Maker,
                    Material = p.Mat,
                    Color_Etch = p.Mat.StartsWith("AL") ? "Black Anodizing" : (p.Mat is "ABS" or "PC" ? "White" : null),
                    Seller = p.Maker == "자체 설계" ? "협력 가공업체" : Pick(sellers),
                    LeadTime = Pick(leadTimes),
                    Stock = stock,
                };
            }).ToList();

            db.Component.AddRange(components);
            await db.SaveChangesAsync();

            // 입출고 이력: 최근 90일, 최근 7일 분량도 포함
            string[] accounts = { "생산팀", "연구1팀", "연구2팀", "A/S", "데모 장비", "고객사 출하" };
            var histories = new List<CompHistory>();
            foreach (var c in components)
            {
                int n = Rng.Next(2, 6);
                for (int i = 0; i < n; i++)
                {
                    var date = i == 0 ? DaysAgo(0, 6) : DaysAgo(7, 90);
                    int roll = Rng.Next(0, 10);
                    string state = roll < 5 ? "입고" : (roll < 9 ? "출고" : "폐기");
                    int qty = state == "입고" ? c.Quantity * Rng.Next(1, 3) : Rng.Next(1, Math.Max(2, c.Quantity / 2 + 1));
                    histories.Add(new CompHistory
                    {
                        ComponentId = c.ComponentId,
                        Date = date,
                        state = state,
                        Quantity = qty,
                        Price = c.Price,
                        Account = state == "입고" ? c.Seller : (state == "폐기" ? "불량 폐기" : Pick(accounts)),
                        EntryDate = date.AddHours(Rng.Next(9, 18)).AddMinutes(Rng.Next(0, 60)),
                    });
                }
            }
            db.CompHistory.AddRange(histories);

            // BOM: 제품별 버전 2개씩
            var boms = new List<Bom>();
            foreach (var product in new[] { "Ordyh", "RUO" })
            {
                var productParts = components.Where(c => c.Product == product).ToList();
                foreach (var (ver, ago) in new[] { ("v1.0", 300), ("v1.1", 60) })
                {
                    var bom = new Bom { ProductName = product, BomVersion = ver, CreatedAt = Today.AddDays(-ago) };
                    // v1.0 은 일부 부품이 빠진 구버전
                    var included = ver == "v1.0" ? productParts.Where((_, idx) => idx % 5 != 4) : productParts;
                    foreach (var c in included)
                    {
                        var spec = parts.First(p => p.Desc == c.Description && p.Product == product);
                        bom.BomDetails.Add(new BomDetail
                        {
                            ComponentId = c.ComponentId,
                            Quantity = spec.BomQty,
                            Price = c.Price,
                            Currency = c.Currency,
                        });
                    }
                    boms.Add(bom);
                }
            }
            db.Bom.AddRange(boms);
            await db.SaveChangesAsync();

            logger.LogInformation("[DemoData] Component {Count}건, 이력 {Hist}건, BOM {Bom}건 생성",
                components.Count, histories.Count, boms.Count);
        }

        // ───────────────────────── 출하 ─────────────────────────
        private static async Task SeedShipmentsAsync(MxTWebContext db, ILogger logger)
        {
            if (await db.Shipment.AnyAsync()) return;

            string[] destinations = { "서울대학교 의과대학", "KAIST 바이오및뇌공학과", "연세대학교 약학대학", "한국생명공학연구원",
                                      "고려대학교 의공학과", "POSTECH 생명과학과", "성균관대학교 약학대학", "아산생명과학연구원",
                                      "Singapore A*STAR (해외)", "Univ. of Tokyo (해외)" };
            string[] chips = { "A01_Small", "A01_Large", "B01_Small", "B01_Large" };

            var list = new List<Shipment>();
            for (int i = 0; i < 18; i++)
            {
                var date = DaysAgo(0, 360);
                list.Add(new Shipment
                {
                    Division = "Chip",
                    Name = "Organ-on-a-Chip",
                    Destination = Pick(destinations),
                    Date = date,
                    LotNumber = $"L{date:yyMMdd}-{Rng.Next(1, 4):D2}",
                    Chip = Pick(chips),
                    Quantity = Rng.Next(1, 11) * 5,
                    Comments = Rng.Next(0, 4) == 0 ? "드라이아이스 동봉" : null,
                });
            }
            for (int i = 0; i < 9; i++)
            {
                var date = DaysAgo(3, 500);
                list.Add(new Shipment
                {
                    Division = "RUO",
                    Name = "Ordyh RUO System",
                    Destination = Pick(destinations),
                    Date = date,
                    SerialNumber = $"ORD-{date:yy}{i + 1:D4}",
                    LotNumber = $"SYS{date:yyMM}",
                    Quantity = 1,
                    SWVersion = Pick(new[] { "1.2.0", "1.3.1", "1.4.0", "2.0.0" }),
                    HWVersion = Pick(new[] { "Rev.B", "Rev.C" }),
                    Comments = Pick(new[] { "설치 교육 완료", "데모 장비 전환", "1년 무상 A/S", null }),
                });
            }

            db.Shipment.AddRange(list);
            await db.SaveChangesAsync();
            logger.LogInformation("[DemoData] Shipment {Count}건 생성", list.Count);
        }

        // ───────────────────────── 칩 사용 / 압력 테스트 / 칩 요청 ─────────────────────────
        private static async Task SeedChipsAsync(MxTWebContext db, ILogger logger)
        {
            if (await db.ChipUsage.AnyAsync() || await db.ChipPressure.AnyAsync()) return;

            string[] chips = { "A01_Small", "A01_Large", "B01_Small", "B01_Large" };
            var usages = new List<ChipUsage>();
            var pressures = new List<ChipPressure>();

            foreach (var chip in chips)
            {
                // 칩 종류별로 오래된 날짜부터 재고를 누적 계산 (제작 → 재고 증가, 납품 → 재고 감소)
                int stock = 0;
                var date = Today.AddDays(-150 - Rng.Next(0, 10));
                int lotSeq = 1;

                while (date <= Today)
                {
                    bool produce = stock < 40 || Rng.Next(0, 3) == 0;
                    string wafer = Rng.Next(0, 2) == 0 ? "SOI" : "SOG";
                    string waferInfo = $"{wafer}-{Rng.Next(1, 26):D2}";

                    if (produce)
                    {
                        int made = Rng.Next(4, 13) * 5;
                        string lot = $"{chip[..3]}{(chip.EndsWith("Small") ? "S" : "L")}-{date:yyMMdd}-{lotSeq++:D2}";
                        usages.Add(new ChipUsage
                        {
                            Chip = chip,
                            Division = "제작",
                            UseDate = date,
                            Producer = Pick(Staff),
                            Wafer = wafer,
                            WaferInfo = waferInfo,
                            Lot = lot,
                            Available = stock,
                            Production = made,
                            Left = stock + made,
                            Note = Rng.Next(0, 5) == 0 ? "본딩 조건 변경 (플라즈마 60s)" : null,
                        });
                        stock += made;

                        // 제작 Lot 별 압력(누설) 테스트 결과
                        int tests = Rng.Next(6, 13);
                        for (int n = 1; n <= tests; n++)
                        {
                            int pressure = Rng.Next(0, 10) == 0 ? Rng.Next(60, 140) : Rng.Next(160, 320);
                            pressures.Add(new ChipPressure
                            {
                                Number = n,
                                Lot = lot,
                                TestDate = date.AddDays(1),
                                ProduceDate = date,
                                TestNum = Rng.Next(1, 4),
                                WaferInfo = waferInfo,
                                Pressure = pressure,
                                passFail = pressure >= 150 ? "PASS" : "FAIL",
                                waferNumber = Rng.Next(1, 26),
                                isUsed = date < Today.AddDays(-30) ? 1 : Rng.Next(0, 2),
                            });
                        }
                    }
                    else
                    {
                        int delivered = Math.Min(stock, Rng.Next(2, 9) * 5);
                        int left = stock - delivered;
                        usages.Add(new ChipUsage
                        {
                            Chip = chip,
                            Division = left == 0 ? "소진" : "납품",
                            UseDate = date,
                            Producer = Pick(Staff),
                            Wafer = wafer,
                            WaferInfo = waferInfo,
                            ReceiptDate = date,
                            Operator = Pick(Departments),
                            Lot = usages.LastOrDefault(u => u.Chip == chip && u.Division == "제작")?.Lot,
                            Available = stock,
                            Production = delivered,
                            Left = left,
                        });
                        stock = left;
                    }

                    date = date.AddDays(Rng.Next(3, 9));
                }
            }

            db.ChipUsage.AddRange(usages);
            db.ChipPressure.AddRange(pressures);

            string[] purposes = { "간 독성 모델 실험", "면역세포 이동 분석", "약물 투과도 테스트", "공동연구 샘플 제작", "신규 프로토콜 검증" };
            var requests = new List<ChipusageRequest>();
            for (int i = 0; i < 12; i++)
            {
                var status = i < 4 ? RequestStatusEnum.Pending : (i < 10 ? RequestStatusEnum.Approved : RequestStatusEnum.Rejected);
                var applied = DaysAgo(0, 40);
                requests.Add(new ChipusageRequest
                {
                    ChipType = Rng.Next(0, 4), // 0:B01_Small 1:B01_Large 2:A01_Small 3:A01_Large
                    Applicant = Pick(Staff),
                    ApplicationDate = applied,
                    RequestedQuantity = Rng.Next(1, 5) * 5,
                    Purpose = Pick(purposes),
                    Status = status,
                    Approver = status == RequestStatusEnum.Pending ? null : "admin@mxtbiotech.com",
                    ProcessedDate = status == RequestStatusEnum.Pending ? null : applied.AddDays(Rng.Next(0, 3)).AddHours(Rng.Next(9, 18)),
                    RejectionReason = status == RequestStatusEnum.Rejected ? "해당 칩 생산 일정 지연" : null,
                    CreatedDate = applied.AddHours(Rng.Next(9, 18)),
                });
            }
            db.ChipUsageRequest.AddRange(requests);

            await db.SaveChangesAsync();
            logger.LogInformation("[DemoData] ChipUsage {Usage}건, ChipPressure {Pressure}건, 칩 요청 {Req}건 생성",
                usages.Count, pressures.Count, requests.Count);
        }

        // ───────────────────────── 업데이트 게시판 (ApplicationDbContext) ─────────────────────────
        private static async Task SeedUpdatePostsAsync(ApplicationDbContext db, ILogger logger)
        {
            if (await db.UpdatePosts.AnyAsync()) return;

            var posts = new List<UpdatePost>
            {
                new() { Title = "Ordyh 제어 SW 1.3.1 배포", Equipment = "Ordyh", Version = "1.3.1",
                        Content = "- 압력 센서 캘리브레이션 오차 수정\n- 로그 내보내기(CSV) 기능 추가", CreatedAt = Today.AddDays(-120), CreatedBy = "admin@mxtbiotech.com" },
                new() { Title = "Ordyh 제어 SW 1.4.0 배포", Equipment = "Ordyh", Version = "1.4.0",
                        Content = "- 다중 채널 유량 제어 지원\n- 온도 PID 튜닝 개선\n- 서버 자동 업로드 연동", CreatedAt = Today.AddDays(-45), CreatedBy = "admin@mxtbiotech.com" },
                new() { Title = "Ordyh 제어 SW 2.0.0 배포", Equipment = "Ordyh", Version = "2.0.0", IsLatest = true,
                        Content = "- UI 전면 개편\n- 실시간 이미지 분석 모듈 추가\n- 장비 상태 원격 모니터링", CreatedAt = Today.AddDays(-6), CreatedBy = "admin@mxtbiotech.com" },
                new() { Title = "압력 테스트 툴 1.1.0", Equipment = "Pressure Tester", Version = "1.1.0", IsLatest = true,
                        Content = "- 측정 결과를 웹 서버(ChipPressure API)로 자동 전송", CreatedAt = Today.AddDays(-20), CreatedBy = "admin@mxtbiotech.com" },
            };

            db.UpdatePosts.AddRange(posts);
            await db.SaveChangesAsync();
            logger.LogInformation("[DemoData] UpdatePost {Count}건 생성", posts.Count);
        }
    }
}
