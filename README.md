# Laplace's Game — 게임 클라이언트 코드

2025년 팀 프로젝트 **Laplace's Game**의 게임 클라이언트 코드입니다. 초기에는 서브, 이후에는 메인 프로그래머로 참여했습니다. 스킬 제작 도구뿐 아니라 캐릭터 전투, 스테이지 진행, 데이터 연결 및 UI 등 개발·수정에 참여한 코드 전반을 담았습니다.

이 저장소는 **코드 열람용**입니다. 원본 Unity 프로젝트의 `DesireGame/Assets/Scripts`를 `Scripts`로 옮겼으며, 별도 데이터 생성 프로그램은 `DataGenerator`에 두었습니다.

## 게임 실행 구조

현재 코드에서는 게임 씬을 초기화한 뒤 스테이지 데이터로 양 팀을 배치합니다. 팀과 베팅 금액을 선택해 전투를 시작하고, 캐릭터 전투 결과를 정산한 뒤 다음 라운드로 진행합니다.

- [GameScene.cs](Scripts/Scenes/GameScene.cs): 게임 씬 초기화와 첫 라운드 시작
- [GameManager.cs](Scripts/System/SingletonObjects/Managers/GameManager.cs): 공통 초기화와 갱신 콜백 관리
- [StageManager.cs](Scripts/System/SingletonObjects/Managers/StageManager.cs): 라운드 상태와 캐릭터 배치·전투 시작·결과 연결
- [RoundSession.cs](Scripts/System/Round/RoundSession.cs): Unity/UI에서 분리한 베팅·골드·정산 상태
- [캐릭터 시스템](Scripts/System/Char): 캐릭터 상태, AI와 행동
- [게임 화면 UI](Scripts/UI/GameSceneUI): 전투 정보, 라운드, 베팅 및 시너지 표시

## 주요 구현과 코드 위치

| 영역 | 구현 내용 | 코드 |
| --- | --- | --- |
| XLSX 데이터 파이프라인 | 시트에서 CSV와 C# 데이터·파싱 코드 생성 | [DataGenerator](DataGenerator), [DataSheets](Scripts/DataSheets) |
| 데이터 로딩 | 생성한 데이터와 게임 시스템 연결 | [DataManager.cs](Scripts/System/SingletonObjects/Managers/DataManager.cs) |
| 캐릭터 제작 | SPUM 원형과 캐릭터 데이터를 선택해 필수 컴포넌트·참조를 구성 | [CharGenerator.cs](Scripts/Tool/CharGenerator/CharGenerator.cs) |
| 스킬 실행 | Timeline Marker의 알림을 받아 스킬 실행 정보 전달 | [SkillMarkerReceiver.cs](Scripts/System/Skill/SkillMarkerReceiver.cs), [Timeline 코드](Scripts/System/TimeLine) |
| 데이터 기반 효과 적용 | Function 인덱스로 데이터를 조회하고 대상별 효과 등록 | [SkillMarkerFunctionInjector.cs](Scripts/System/TimeLine/Marker/SkillMarkerFunctionInjector.cs), [Function 코드](Scripts/System/Function) |
| 개별 효과 검증 | 시전자·대상·Function을 선택해 직접 주입 | [CharFunctionInjector.cs](Scripts/Tool/InGameTool/CharFunctionInjector.cs) |
| 스킬·전투 검증 | 캐릭터 배치와 일반 공격·스킬 개별 실행 | [BattleTestTool.cs](Scripts/Tool/InGameTool/BattleTestTool.cs) |
| 스테이지 검증 | 스테이지 ID로 전투 구성을 불러오고 실행 | [StageTool.cs](Scripts/Tool/InGameTool/StageTool.cs) |
| 시너지 | 팀별 시너지 집계·적용과 UI 연결 | [SynergyManager.cs](Scripts/System/SingletonObjects/Managers/SynergyManager.cs), [Synergy 코드](Scripts/System/Synergy) |
| 테스트 | 라운드 정산과 시너지 UI 등 확인용 테스트 | [Scripts/Tests](Scripts/Tests) |

## 스킬 제작과 검증

Timeline에서 실행 시점을 지정하고 Marker에 Function 인덱스와 대상 선택 방식을 설정합니다. `SkillMarkerReceiver`가 전달한 시전자·대상 정보를 바탕으로 Marker가 효과를 적용하거나 투사체 등 개별 동작을 실행합니다.

효과 자체를 확인할 때는 Timeline 전체를 재생하지 않고 `CharFunctionInjector`로 직접 주입할 수 있습니다. 일반 공격·스킬의 실행은 `BattleTestTool`, 스테이지 구성의 확인은 `StageTool`로 나누었습니다.

## 버전과 구현 범위

- 팀 개발 당시 코드와 2026년 후속 보완 코드가 함께 포함되어 있습니다. 특히 현재의 라운드·베팅 및 시너지 UI 관련 코드를 모두 2025년 당시 완성한 기능으로 소개하지 않습니다.
- 시너지 관련 소스도 구현 현황을 보여주기 위해 포함했습니다. 모든 기획 시너지와 콘텐츠가 완성·검증되었다는 의미는 아닙니다.
- 팀 프로젝트의 코드이므로 모든 파일의 단독 저작을 주장하지 않습니다. 포트폴리오의 담당 범위와 관련 코드를 중심으로 확인할 수 있도록 경로를 정리했습니다.

## 실행 제한과 의존성

- 아트·음원·씬·프리팹·애니메이션·Timeline 에셋·게임 데이터 원본 및 외부 라이브러리 본체는 포함하지 않았습니다.
- Unity Timeline, TextMesh Pro, NavMesh 관련 기능과 SPUM 원형 프리팹 등에 의존합니다. 소스만 Unity에 복사해 실행할 수 있는 독립 프로젝트는 아닙니다.
- `DataGenerator`에는 현재 `Program.cs`와 `DataFormat.cs`가 들어 있습니다. XLSX 입력, 프로젝트·패키지 설정과 원본 출력 디렉터리가 빠져 있으므로 그대로 빌드·실행하는 배포 패키지가 아닙니다. 코드에서 ExcelDataReader와 Spire.XLS를 사용합니다.
- 테스트 코드는 포함했지만, 이 리소스 제외본에서 테스트를 실행하거나 통과를 검증했다는 의미는 아닙니다.
- 원본의 외부 의존성과 팀 코드의 이용 조건을 변경하는 별도 라이선스는 부여하지 않습니다.
