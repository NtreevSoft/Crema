Ntreev.Crema.ServiceHosts.Http
===============================
이 프로젝트는 크레마 서버 연동을 위해 HTTP 프로토콜을 이용하여 REST API 를 제공합니다.

## 사용 방법
크레마 서버 실행 시 HTTP 서비스도 함께 제공합니다.  
기본적으로 HTTP 서비스 포트는 크레마 서버 포트에 100을 더한 포트로 실행합니다.  
HTTP 포트를 변경하기 위해 `--http-port` 옵션을 참고하십시오.

만약 HTTP 서비스를 사용하지 않으려면 아래와 같이 `--no-http-server` 옵션을 지정하십시오.  
더 자세한 내용은 `cremaserver.exe help run` 을 실행하십시오.

```bash
cremaserver.exe run <...> [--http-port] [--no-http-server]
```

## Swagger 통합
크레마 HTTP API 는 Swagger 문서와 UI 를 통합합니다.  

### Swagger UI, Docs
크레마 HTTP API 서버 포트가 4104 인 경우 아래와 같이 Swagger 를 사용할 수 있습니다.

- Swagger UI: http://localhost:4104/swagger
- Swagger Docs: http://localhost:4104/swagger/docs/v1

### Swagger UI 사용자 로그인

크레마 HTTP API 를 테스트 하기 위해 Swagger UI 에 로그인 하는 방법입니다.

1. 크레마 Swagger UI 로 로그인 합니다.
2. **오른쪽 상단의 `login`** 버튼을 클릭합니다.
3. 크레마 사용자 로그인 URL, 사용자 아이디, 비밀번호를 입력합니다.
4. 이제 크레마 HTTP API 를 테스트 할 수 있습니다.

## 디자인 가이드
Crema HTTP API v1 디자인 사양은 `cremaconsole.exe` 에서 제공하는 자바스크립트 함수를 REST API 로 이식하는데 있습니다. 초기 v1 디자인은 다음 REST API 버전에서 변경될 수 있습니다.

HTTP API 는 기본적으로 아래와 같은 HTTP 동사를 사용합니다.

- 조회 명령은 HTTP GET 동사를 이용합니다.
- 생성 명령은 HTTP POST 동사를 이용합니다.
- 업데이트 명령은 HTTP PUT 동사를 이용합니다.
- 삭제 명령은 HTTP DELETE 동사를 이용합니다.

하지만 다음의 경우 예외적으로 다른 HTTP 동사를 사용합니다.

- TableItem, TypeItem, UserItem 은 GET URL 로 표현할 수 없습니다.  
  예) DELETE http://<host>/api/v1/commands/table-item/\<table-item-path\>  
  `'<table-item-path>'` 의 값은 `'/GameItem/Items'` 와 같이 슬래시(/) 문자를 포함하고 있습니다.  
  이런 경우 HTTP POST 또는 HTTP PUT 동사를 사용합니다.