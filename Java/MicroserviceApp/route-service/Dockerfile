FROM eclipse-temurin:17-jre
WORKDIR /opt/app
COPY route-service/target/route-service-1.0.0.jar app.jar
ENTRYPOINT ["java", "-Dserver.address=0.0.0.0", "-jar", "app.jar"]
