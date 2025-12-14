FROM eclipse-temurin:17-jre
WORKDIR /opt/app
COPY city-service/target/city-service-1.0.0.jar app.jar
ENTRYPOINT ["java", "-Dserver.address=0.0.0.0", "-jar", "app.jar"]
