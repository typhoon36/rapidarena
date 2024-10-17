# rapidarena

## 게임 소개

- 장르 : 빠른 페이스 슈팅게임(트위치 슈터(high-octane))

- 개발기간 (2024.10.3 → Ing)

- 목적  : 서버를 통해 관리하는 카운터스트라이크같은 게임을 만들고자했습니다.

- 관리 : Github & Jira




## 개발 환경
- 플랫폼 : Windows 11

- 언어 : C#

- 엔진 환경 : Unity 2022.03.15(LTS)




## 사용 기술

| 항목 | 설명 |
| ------------ | ------------- |
| 디자인 패턴 | - 싱글톤 패턴을 사용해서 전역 접근 관리
               -  State Pattern을 사용해 캐릭터 애니메이션을 객체 관리 |
| Object Pooling | 자주 사용되는 객체는 재사용하여 관리 |
| FSM | 몬스터들의 패턴을 직관적으로 관리 |
| 직렬화 | 유니티상에서 클래스를 직렬화하여 작업 편의성 증대 |
| 코루틴  | 연출은 코루틴으로 관리하여 메모리 확보 |
| Save | 게임내 데이터를 글로벌 변수에 저장 및 관리 |


## 구현 기능

* Player
  * 
        

 ## 기술 문서
[기술 문서](https://docs.google.com/presentation/d/15tD-nxOH4juBukBCzeHNn3QBTxk8-W_Du52wTj0wxL0/edit#slide=id.g301078c2494_0_176)


 ## velog

[블로그](https://velog.io/@typhoon760/posts?tag=%ED%8F%AC%ED%8A%B8%ED%8F%B4%EB%A6%AC%EC%98%A4)

 ## 영상

 [플레이 영상](https://youtu.be/B-tIQGz2CaY)
