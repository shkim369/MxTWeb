# MxTWeb 프로젝트

## 프로젝트 개요
- Blazor Server 웹 애플리케이션
- .NET 9.0 기반
- MudBlazor UI 프레임워크 사용

## 기술 스택
- **프레임워크**: ASP.NET Core Blazor
- **UI**: MudBlazor 8.x
- **데이터베이스**: SQL Server + Entity Framework Core 9
- **인증**: ASP.NET Core Identity
- **이메일**: MailKit, SendGrid
- **엑셀**: ClosedXML
- **컨테이너**: Docker (Linux)

## 프로젝트 구조
```
MxTWeb/
├── Components/       # Blazor 컴포넌트
│   ├── Layout/       # 레이아웃 컴포넌트
│   └── Pages/        # 페이지 컴포넌트
├── Data/             # DbContext, 엔티티
├── Services/         # 비즈니스 로직 서비스
└── wwwroot/          # 정적 파일
```

## 코딩 컨벤션
- 한국어 주석 허용
- PascalCase: 클래스, 메서드, 속성
- camelCase: 지역 변수, 매개변수
- _camelCase: private 필드

## 중요 사항
- 민감한 설정은 User Secrets 또는 환경 변수 사용
- 새 페이지 추가 시 NavMenu.razor에 메뉴 항목 추가 필요

## 자주 사용하는 명령어
```bash
# 프로젝트 실행
dotnet run --project MxTWeb

# 마이그레이션 추가
dotnet ef migrations add [MigrationName] --project MxTWeb

# 데이터베이스 업데이트
dotnet ef database update --project MxTWeb
```

