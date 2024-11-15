# rapidarena

## 게임 소개

- 장르 : 빠른 페이스 슈팅게임(트위치 슈터(high-octane))

- 개발기간 (2024.10.3 → Ing)

- 목적  : 카운터스트라이크같은 게임을 만들고자했습니다.

- 관리 : Github & Jira




## 개발 환경
- 플랫폼 : Windows 11

- 언어 : C#

- 엔진 환경 : Unity 2022.03.15(LTS)




## 사용 기술

| 항목 | 설명 |
| ------------ | ------------- |
| 디자인 패턴 | 싱글톤 패턴을 사용해서 전역 접근 관리 |
| Object Pooling | 자주 사용되는 객체는 Pool로 관리하여 재사용|
| Save | 게임 내 데이터를 JSON으로 변환해 저장 및 로드|
| Photon| 포톤을 이용한 멀티플레이 기능 관리 |


## 구현 기능

* Player
  * 팀 별 전투
   
* Scene
 * 로비
 * 룸
 * 상점
 * 인벤토리

* UI
 * Scene
     * common UI - 로비로 이동,종료,설정
     * WorldSpace - 닉네임
   * InGame  : HP,총기의 상태,팀 우승 상태
   * Inven : 인벤 슬롯, 인벤 추가, 인벤 버리기 버튼  
   * Shop : 판매버튼, 간이 인벤 슬롯, 상품 관리
 ## 기술 문서
[기술 문서](https://docs.google.com/presentation/d/1ASBFL0deqHO50BUfyGx-w9X3S-U0FXeQ4qpRaYTWGsU/edit#slide=id.g301078c2494_0_171)


 ## velog

[블로그](https://velog.io/@typhoon760/posts?tag=%ED%8F%AC%ED%8A%B8%ED%8F%B4%EB%A6%AC%EC%98%A4)


