package com.example.route.domain;

import com.example.city.domain.City;
import jakarta.persistence.*;
import java.time.LocalDate;

@Entity
@Table(name = "connections", indexes = {
        @Index(name = "ix_conn_from", columnList = "from_city_id"),
        @Index(name = "ix_conn_to", columnList = "to_city_id")
})
public class Connection {
    @Id
    @Column(name = "id", nullable = false, updatable = false, length = 64)
    private String id;

    @ManyToOne(optional = false, fetch = FetchType.LAZY)
    @JoinColumn(name = "from_city_id", referencedColumnName = "id", nullable = false)
    private City from;

    @ManyToOne(optional = false, fetch = FetchType.LAZY)
    @JoinColumn(name = "to_city_id", referencedColumnName = "id", nullable = false)
    private City to;

    @Column(name = "valid_from", nullable = false)
    private LocalDate validFrom;

    @Column(name = "valid_to", nullable = false)
    private LocalDate validTo;

    public Connection() {}
    public String getId() { return id; }
    public City getFrom() { return from; }
    public City getTo() { return to; }

    public LocalDate getValidFrom() { return validFrom; }
    public void setValidFrom(LocalDate validFrom) { this.validFrom = validFrom; }

    public LocalDate getValidTo() { return validTo; }
    public void setValidTo(LocalDate validTo) { this.validTo = validTo; }

}
